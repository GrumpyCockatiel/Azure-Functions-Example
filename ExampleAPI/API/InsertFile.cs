using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Raydreams.API.Example
{
    /// <summary>Uploads binary data to Blob Storage</summary>
    public class InsertFileFunction : BaseFunction
    {
        public InsertFileFunction(IDataGateway gate) : base(gate)
        { }

        [Function("InsertFile")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/file/{fileName?}")] HttpRequestData req, string fileName, FunctionContext ctx)
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