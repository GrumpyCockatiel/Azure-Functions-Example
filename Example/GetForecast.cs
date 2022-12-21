using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Raydreams.API.Example.Extensions;
using Raydreams.API.Example.Model;
using System.Collections.Generic;

namespace Raydreams.API.Example
{
    /// <summary></summary>
    public class GetForecastFunction : BaseFunction
    {
        public GetForecastFunction( IDataGateway gate ) : base( gate )
        { }

        [Function("GetForecast")]
        public async Task<HttpResponseData> Run( [HttpTrigger( AuthorizationLevel.Anonymous, "get", Route = "v1/forecast" )] HttpRequestData req, FunctionContext ctx )
        {

            // log that this function was triggered
            ILogger logger = ctx.GetLogger( "API" );
            logger.LogInformation( $"{GetType().Name} triggered." );

            APIResult<IEnumerable<ForecastDTO>> results = new APIResult<IEnumerable<ForecastDTO>>();

            try
            {
                //var starting = new DateOnly(year, month, day);

                //if (starting > DateOnly.FromDayNumber( DateTimeOffset.UtcNow.AddDays(10).DayOfYear) )
                   // req.BadResponse("Can't predict more than 10 days from now");

                this.Gateway.AddHeaders( req );
                results.ResultObject = await this.Gateway.GetWeather();
            }
            catch ( System.Exception exp )
            {
                return req.ReponseError( results, exp, logger );
            }

            if ( results.ResultObject != null )
                results.ResultCode = APIResultType.Success;

            return req.OKResponse( results );
        }
    }
}

