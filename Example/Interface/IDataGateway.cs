using System;
using Microsoft.Azure.Functions.Worker.Http;
using Raydreams.API.Example.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Raydreams.API.Example
{
    /// <summary>Gateway Interface</summary>
	public interface IDataGateway
    {
        /// <summary></summary>
        IDataGateway AddHeaders( HttpRequestData req );

        /// <summary></summary>
        string Login( OAuthState state );

        /// <summary></summary>
        string Logout();

        /// <summary></summary>
        Task<TokenResponse> GetToken( string code, OAuthState state = null );

        /// <summary></summary>
        string Ping( string msg );

        /// <summary></summary>
        Task<IEnumerable<WeatherForecast>> GetWeather( DateOnly day );
    }
}
