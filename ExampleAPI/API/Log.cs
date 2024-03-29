﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raydreams.API.Example.Extensions;

namespace Raydreams.API.Example
{
    /// <summary>Writes a message to the logger</summary>
    public class LogFunction : BaseFunction
    {
        public LogFunction( IDataGateway gate ) : base( gate )
        { }

        [Function( "Log" )]
        public async Task<HttpResponseData> Run( [HttpTrigger( AuthorizationLevel.Anonymous, "post", Route = "v1/log" )] HttpRequestData req, FunctionContext ctx )
        {
            // log that this function was triggered
            ILogger logger = ctx.GetLogger( "API" );
            logger.LogInformation( $"{GetType().Name} triggered." );

            APIResult<bool> results = new APIResult<bool>();

            try
            {
                // get all the values
                string requestBody = await req.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject( requestBody );
                string msg = data?.message;
                string lvl = data?.level;
                string category = data?.category;

                // replace the category if one was sent
                if ( !String.IsNullOrWhiteSpace( category ) )
                    logger = ctx.GetLogger( category );

                Dictionary<string, string> args = new Dictionary<string, string>();

                // check for args
                if ( data.args != null )
                {
                    foreach ( JProperty pi in data.args.Properties() )
                    {
                        string value = pi.Value?.ToString();
                        args.Add( pi.Name, ( !String.IsNullOrWhiteSpace( value ) ) ? value : String.Empty );
                    }
                }

                LogLevel level = lvl.GetEnumValue<LogLevel>( LogLevel.Information );

                this.Gateway.AddHeaders( req );
                logger.Log( level, msg, args.Values.ToArray() );

                results.ResultObject = true;
                results.ResultCode = APIResultType.Success;

                return req.OKResponse( results );
            }
            catch ( Exception exp )
            {
                logger.LogError( exp, null, null );
                return req.ReponseError( exp, logger );
            }
        }
    }
}
