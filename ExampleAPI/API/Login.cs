using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Raydreams.API.Example
{
    /// <summary>The user login function</summary>
    /// <remarks>This is only to roll a login URL and potentially store login attempts</remarks>
    public class LoginFunction : BaseFunction
    {
        public LoginFunction( IDataGateway gate ) : base( gate )
        { }

        /// <param name="id">The client ID to associate the login request with</param>
        /// <returns>A redirect to the Autodesk Login</returns>
        [Function( "Login" )]
        public HttpResponseData Run( [HttpTrigger( AuthorizationLevel.Anonymous, "get", Route = "login/{id?}" )] HttpRequestData req, string id, FunctionContext ctx )
        {
            ILogger logger = ctx.GetLogger( "API" );
            logger.LogInformation( $"{GetType().Name} triggered." );

            APIResult<string> results = new APIResult<string>();

            try
            {
                this.Gateway.AddHeaders( req );

                // get some optional params to pass in state
                int port = req.GetIntValue("p", 50001);
                TokenRetrieveType method = req.GetStringValue("m", "polling").GetEnumValue<TokenRetrieveType>(true);

                // set scopes
                //string scopesStr = req.GetStringValue( "s" ); // scopes

                // if the client id passed in is missing just makes some random ID
                OAuthState state = new OAuthState( method, port, id );

                // add requested scope to the state
                //if ( Int32.TryParse( scopesStr, out int scopes ) )
                    //state.Scopes = (ForgeScopes)scopes;

                // redirect to login
                results.ResultObject = this.Gateway.Login(state);
                //return req.Redirect( url );
            }
            catch ( System.Exception exp )
            {
                //APIResult<bool> results = new APIResult<bool>() { ResultCode = APIResultType.Exception };
                return req.ReponseError( results, exp, logger );
            }

            HttpResponseData resp = req.CreateResponse( HttpStatusCode.OK );
            resp.WriteString( results.ResultObject, Encoding.UTF8 );
            return resp;

            //return req.OKResponse( results );
        }
    }
}

