using System;
using System.IO;
using Microsoft.Azure.Functions.Worker.Http;

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
        Task<IEnumerable<ForecastDTO>> GetWeather();

        /// <summary></summary>
        Task<string> InsertFile( Stream fs, string filename );
    }
}
