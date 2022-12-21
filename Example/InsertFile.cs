﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Raydreams.API.Example.Data;
using Raydreams.API.Example.Extensions;
using Raydreams.API.Example.Model;

namespace Raydreams.API.Example
{
    /// <summary>Uploads binary data to Blob Storage</summary>
    public class InsertImageFunction : BaseFunction
    {
        public InsertImageFunction(IDataGateway gate) : base(gate)
        { }

        [Function("InsertFile")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/file/{fileName?}")] HttpRequestData req, string fileName, FunctionContext ctx)
        {
            ILogger logger = ctx.GetLogger("API");
            logger.LogInformation($"{GetType().Name} triggered.");

            if (String.IsNullOrWhiteSpace(fileName))
                return req.BadResponse("A full filename with extension is required");

            APIResult<string> results = new APIResult<string>() { ResultCode = APIResultType.Unknown };

            try
            {
                var reqData = await ctx.GetHttpRequestDataAsync();
                var file = reqData.Body;

                // upload the file - container name is hard coded in config for now
                results.ResultObject = await this.Gateway.InsertFile(file, fileName.Trim());
            }
            catch (Exception exp)
            {
                return req.ReponseError(results, exp, logger);
            }

            if (!String.IsNullOrWhiteSpace(results.ResultObject))
                results.ResultCode = APIResultType.Success;

            return req.OKResponse(results);
        }
    }

}