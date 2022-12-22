using System.Security.Claims;
using Newtonsoft.Json;

namespace Raydreams.API.Example.Data
{
    /// <summary></summary>
    public class Auth0Manager
    {
        /// <summary></summary>
        private string _secret;

        /// <summary></summary>
        public Auth0Manager(string clientID, string secret, string tenant)
        {
            this.ClientID = clientID;
            this._secret = secret;
            this.TenantURL = tenant;
        }

        /// <summary></summary>
        public string ClientID { get; set; }

        /// <summary></summary>
        public string TenantURL { get; set; }

        /// <summary>Generate a login URL to Auth0</summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Login( string url ) => $"{TenantURL}/authorize?client_id={ClientID}&redirect_uri={url}&response_type=code&scope=openid%20profile%20email%20offline_access";

        /// <summary>Logout URL</summary>
        public string Logout => $"{TenantURL}/v2/logout&client_id={this.ClientID}&returnTo=";

        /// <summary>Gets a token</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<Auth0Token> GetToken(string code, string redirect)
        {
            if ( String.IsNullOrWhiteSpace( code ) )
                return null;

            Auth0Token results = null;

            string body = $"grant_type=authorization_code&client_id={ClientID}&client_secret={_secret}&code={code.Trim()}&redirect_uri={redirect}";

            HttpRequestMessage message = new HttpRequestMessage( HttpMethod.Post, $"{TenantURL}/oauth/token" );
            message.Headers.Clear();
            message.Content = new StringContent( body, Encoding.UTF8, "application/x-www-form-urlencoded" );
            byte[] bytes = Encoding.UTF8.GetBytes( body );
            message.Content.Headers.Add( "Content-Length", bytes.Length.ToString() );

            try
            {
                HttpResponseMessage httpResponse = await new HttpClient().SendAsync( message ); ;
                string response = await httpResponse.Content.ReadAsStringAsync();

                // deserialize the response
                results = JsonConvert.DeserializeObject<Auth0Token>( response );
            }
            catch ( System.Exception )
            {
                return null;
            }

            return results;
        }

        /// <summary></summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Auth0User> GetUserInfo( string token )
        {
            if ( String.IsNullOrWhiteSpace( token ) )
                return null;

            Auth0User results = null;

            HttpRequestMessage message = new HttpRequestMessage( HttpMethod.Get, $"{TenantURL}/userinfo" );
            message.Headers.Clear();
            message.Headers.Authorization = new AuthenticationHeaderValue( "Bearer", token);

            try
            {
                HttpResponseMessage httpResponse = await new HttpClient().SendAsync( message ); ;
                string response = await httpResponse.Content.ReadAsStringAsync();

                // deserialize the response
                results = JsonConvert.DeserializeObject<Auth0User>( response );
            }
            catch ( System.Exception )
            {
                return null;
            }

            return results;
        }

        /// <summary></summary>
        /// <param name="token"></param>
        /// <returns></returns>
        //public async Task<bool> Logout()
        //{
        //    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"{TenantURL}/v2/logout&client_id={this.ClientID}&returnTo=");
        //    message.Headers.Clear();

        //    try
        //    {
        //        HttpResponseMessage httpResponse = await new HttpClient().SendAsync(message); ;
        //        string response = await httpResponse.Content.ReadAsStringAsync();
        //    }
        //    catch (System.Exception)
        //    {
        //        return false;
        //    }

        //    return true;
        //}
    }
}
