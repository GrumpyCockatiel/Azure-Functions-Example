using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Raydreams.API.Example
{
    /// <summary>A simple ping function to test that the APIs are running.</summary>
    public class PingFunction : BaseFunction
    {
        public PingFunction(IDataGateway gate) : base(gate)
        { }

        [Function("Ping")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/ping")] HttpRequestData req, FunctionContext ctx)
        {
            // log that this function was triggered
            ILogger logger = ctx.GetLogger("API");
            logger.LogInformation($"{GetType().Name} triggered.");

            APIResult<string> results = new APIResult<string>();

            try
            {
                // the test message is optional
                string msg = req.GetStringValue("msg");

                this.Gateway.AddHeaders(req);
                results.ResultObject = this.Gateway.Ping(msg);
            }
            catch (System.Exception exp)
            {
                return req.ReponseError(results, exp, logger);
            }

            if (results.ResultObject != null)
                results.ResultCode = APIResultType.Success;

            return req.OKResponse(results);
        }
    }
}
