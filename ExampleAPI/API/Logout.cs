using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Raydreams.API.Example
{
    /// <summary>Chains the logout call specific to this auth client</summary>
    public class LogoutFunction : BaseFunction
    {
        public LogoutFunction( IDataGateway gate ) : base( gate )
        { }

        [Function( "Logout" )]
        public HttpResponseData Run( [HttpTrigger( AuthorizationLevel.Anonymous, "get", Route = "v1/logout" )] HttpRequestData req, FunctionContext ctx )
        {
            ILogger logger = ctx.GetLogger( "API" );
            logger.LogInformation( $"{GetType().Name} triggered." );

            try
            {
                // setup the gateway
                this.Gateway.AddHeaders( req );

                return req.Redirect( this.Gateway.Logout() );
            }
            catch ( System.Exception exp )
            {
                APIResult<bool> results = new APIResult<bool>() { ResultCode = APIResultType.Exception };
                return req.ReponseError<bool>( results, exp, logger );
            }

        }
    }
}

