using System;
using Newtonsoft.Json;

namespace Raydreams.API.Example.Model
{
    /// <summary>Object when responding with token information back to the client</summary>
    public class TokenResponse
    {
        [JsonProperty( "token" )]
        public string Token { get; set; }

        [JsonProperty( "userID" )]
        public string UserID { get; set; }

        [JsonProperty( "refresh" )]
        public string RefreshToken { get; set; }

        [JsonProperty( "expires" )]
        public DateTimeOffset Expires { get; set; }
    }
}

