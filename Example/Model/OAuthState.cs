using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using Raydreams.API.Example.Extensions;

namespace Raydreams.API.Example.Model
{
    /// <summary>How does the client want to get the token info</summary>
    public enum TokenRetrieveType
    {
        /// <summary>The client will poll for it's token</summary>
        Polling = 0,
        /// <summary>Send a localhost redirect back</summary>
        Redirect = 1
    }

    /// <summary></summary>
    public class OAuthState
    {
        /// <summary>Minimum port range to allow</summary>
        public const int MinPort = 5001;

        /// <summary>Max port range to allow</summary>
        public const int MaxPort = 65535;

        private int _port = 0;

        private string _id;

        /// <summary>Parameterless constructor</summary>
        public OAuthState()
        {
        }

        /// <summary>Construct from properties</summary>
        /// <param name="appID"></param>
        /// <param name="type"></param>
        /// <param name="port"></param>
        public OAuthState( TokenRetrieveType type = TokenRetrieveType.Redirect, int port = 5005 )
        {
            // appID is required - we've haven not chosen a required format
            //this.AppID = ( !String.IsNullOrWhiteSpace( appID ) ) ? appID : new Randomizer().YouTubeID();

            this.Method = type;
            this.Port = port;
        }

        /// <summary>Port to use with a redirect</summary>
        /// <remarks>Optional</remarks>
        [JsonProperty( "port" )]
        public int Port
        {
            get => this._port;
            set
            {
                this._port = Math.Clamp( value, MinPort, MaxPort );
            }
        }

        /// <summary>How the client gets the token</summary>
        /// <remarks>Optional</remarks>
        [JsonProperty( "type" )]
        [JsonConverter( typeof( StringEnumConverter ) )]
        public TokenRetrieveType Method { get; set; } = TokenRetrieveType.Redirect;

        /// <summary>Encodes to BASE64UrlEncoding</summary>
        /// <returns></returns>
        public string Encode()
        {
            byte[] bytes = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( this ) );
            return DataTypeConverter.BASE64UrlEncode( bytes );
        }

        /// <summary>Encodes an object</summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static OAuthState Decode( string encoded )
        {
            if ( String.IsNullOrWhiteSpace( encoded ) )
                return null;

            byte[] bytes = DataTypeConverter.BASE64UrlDecode( encoded );
            string json = Encoding.UTF8.GetString( bytes );

            // catch a deserialize error
            return JsonConvert.DeserializeObject<OAuthState>( json );
        }
    }
}
