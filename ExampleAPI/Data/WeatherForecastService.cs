using Newtonsoft.Json;

namespace Raydreams.API.Example.Data
{
    /// <summary></summary>
    public interface IForecastService
    {
        /// <summary>Starting from the given date returns the next 5 days of forecasts</summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        Task<ForecastDTO[]> GetForecastAsync();
    }

    /// <summary>Generates random data</summary>
    /// <remarks>Could move this to the randomizer</remarks>
    public class MockForecastService : IForecastService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<ForecastDTO[]> GetForecastAsync()
        {
            DateOnly startDate = DateOnly.FromDayNumber( DateTimeOffset.UtcNow.DayOfYear );

            return Task.FromResult( Enumerable.Range( 1, 5 ).Select( index => new ForecastDTO
            {
                Date = startDate.AddDays( index ),
                TemperatureC = Random.Shared.Next( -20, 55 ),
                Summary = Summaries[Random.Shared.Next( Summaries.Length )]
            } ).ToArray() );
        }
    }

    /// <summary></summary>
    /// <remarks>Hard coded forecast url
    /// This is far from complete
    /// https://api.weather.gov/gridpoints/HGX/65,97/forecast
    /// https://www.weather.gov/documentation/services-web-api#/
    /// </remarks>
    public class NWSForecastService : IForecastService
    {
        /// <summary>The user agent name to use in the requests</summary>
        public string UserAgent { get; set; } = "PostmanRuntime/7.28.4";

        public string Station { get; set; } = String.Empty;

        public string Grid { get; set; } = String.Empty;

        /// <summary></summary>
        /// <returns></returns>
        /// <remarks>NWS requires you set the User Agent</remarks>
        public async Task<ForecastDTO[]> GetForecastAsync()
        {
            List<ForecastDTO> results = new List<ForecastDTO>();

            HttpRequestMessage message = new HttpRequestMessage( HttpMethod.Get, $"https://api.weather.gov/gridpoints/{Station}/{Grid}/forecast?units=si" );
            message.Headers.Clear();
            message.Headers.Add( "User-Agent", this.UserAgent );

            // hold the reponse
            WeatherForecast resp = new WeatherForecast();

            try
            {
                HttpResponseMessage httpResponse = await new HttpClient().SendAsync( message );

                var statusCode = httpResponse.StatusCode;
                string rawResponse = await httpResponse.Content.ReadAsStringAsync();

                // deserialize the response
                resp = JsonConvert.DeserializeObject<WeatherForecast>( rawResponse );

            }
            catch ( System.Exception exp )
            {
                //this.Logger.Log(exp);
            }

            if ( resp.properties != null && resp.properties.periods != null )
            {
                foreach ( Period p in resp.properties.periods )
                {
                    results.Add(new ForecastDTO
                    {
                        TemperatureC = p.temperature,
                        Date = DateOnly.FromDateTime(p.startTime),
                        Summary = p.detailedForecast
                    });
                }
            }

            return results.ToArray();
        }
    }
}
