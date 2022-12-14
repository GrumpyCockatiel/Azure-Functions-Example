using System;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Raydreams.API.Example.Model
{
    /// <summary>An Auth0 User</summary>
    public class Auth0User // : IdentityUser
    {
        /// <summary>The primary token</summary>
        [JsonProperty( "sub" )]
        public string ID { get; set; }

        [JsonProperty( "name" )]
        public string Name { get; set; }

        [JsonProperty( "email" )]
        public string Email { get; set; }

        [JsonProperty( "email_verified" )]
        public bool Verified { get; set; }

        [JsonProperty( "updated_at" )]
        public DateTimeOffset UpdatedAt { get; set; }

        public Claim[] Claims => new[] { new Claim( ClaimTypes.Email, this.Email ),
            new Claim( ClaimTypes.Name, this.Name ) };
    }
}

