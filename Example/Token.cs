using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Raydreams.API.Example.Extensions;
using Raydreams.API.Example.Model;

namespace Raydreams.API.Example
{
    /// <summary></summary>
    public class CallbackFunction : BaseFunction
    {
        public CallbackFunction( IDataGateway gate ) : base( gate )
        { }

        [Function( "Token" )]
        public async Task<HttpResponseData> Run( [HttpTrigger( AuthorizationLevel.Anonymous, "get", Route = "token" )] HttpRequestData req, FunctionContext ctx )
        {
            ILogger logger = ctx.GetLogger( "API" );
            logger.LogInformation( $"{GetType().Name} triggered." );

            // the OAuth Code
            string code = req.GetStringValue( "code" );

            // always need a code though we could respond to tell the client something happend
            if ( string.IsNullOrWhiteSpace( code ) )
                return req.BadResponse( "Invalid input parameters." );

            // the original state data passed by the client - an encoded object that also says HOW to repond
            string stateStr = req.GetStringValue( "state" );

            // unravel the state param
            OAuthState state = OAuthState.Decode( stateStr );

            APIResult<TokenResponse> results = new APIResult<TokenResponse>() { ResultCode = APIResultType.Unauthorized };

            try
            {
                // setup the gateway
                this.Gateway.AddHeaders( req );

                if ( state.Port < OAuthState.MinPort )
                    state.Port = 50001;

                // now get the final JWT
                results.ResultObject = await this.Gateway.GetToken( code, state );
            }
            catch ( Exception exp )
            {
                return req.ReponseError<TokenResponse>( results, exp, logger );
            }

            // if method is redirect - tell the client to redirect again
            if ( state.Method == TokenRetrieveType.Redirect )
            {
                string query = $"login?token={results.ResultObject.Token}&refresh={results.ResultObject.RefreshToken}&userID={results.ResultObject.UserID}&expires={results.ResultObject.Expires}";

                return req.Redirect( $"http://{IPAddress.Loopback}:{state.Port}/{query}" );
            }
            else // echo back the object in the response which has the token
            {
                if (results.ResultObject != null && !String.IsNullOrWhiteSpace(results.ResultObject.Token))
                    results.ResultCode = APIResultType.Success;

                return req.OKResponse<TokenResponse>( results );
            }

        }
    }
}

