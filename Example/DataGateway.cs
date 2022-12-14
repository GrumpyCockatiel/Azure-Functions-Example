using System;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Raydreams.API.Example.Extensions;
using Raydreams.API.Example.Model;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Raydreams.API.Example.Data;
using System.Threading.Tasks;

namespace Raydreams.API.Example
{
    /// <summary>Gateway Interface</summary>
	public interface IDataGateway
	{
        IDataGateway AddHeaders(HttpRequestData req);

        string Login(OAuthState state);

        Task<TokenResponse> GetToken(string code, OAuthState state = null);

        string Ping(string msg);

        int GetSomeBackendData(int id);
	}

    /// <summary></summary>
	public class DataGateway : IDataGateway
	{
        #region [ Fields ]

        /// <summary>Session timeout (secs) defaults to one hour</summary>
        private int _defaultTimeout = 3600 * 1;

        #endregion [ Fields ]

        /// <summary></summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public DataGateway( EnvironmentSettings settings, ILogger<DataGateway> logger )
		{
            this.Config = settings;
            this.Logger = logger;
            this.AuthManager = new Auth0Manager(this.Config.IDClientID, this.Config.IDClientSecret, this.Config.TenantURL);
        }

        #region [ Properties ]

        /// <summary>Store the client agent</summary>
        protected string ClientAgent { get; set; } = "no client";

        /// <summary>Store the Client IP</summary>
        protected string ClientIP { get; set; } = "0.0.0.0";

        /// <summary>Client requested URL</summary>
        protected Uri RequestedURL { get; set; }

        /// <summary></summary>
        protected EnvironmentSettings Config { get; set; }

        /// <summary></summary>
        protected Auth0Manager AuthManager { get; set; }

        /// <summary>The default logger</summary>
        public ILogger<DataGateway> Logger { get; set; }

        #endregion [ Properties ]

        /// <summary>Allows adding additional headers after the class is created</summary>
        /// <param name="req">The request that if null will set to default client header values</param>
        public IDataGateway AddHeaders(HttpRequestData req)
        {
            ClientHeaders headers = req.GetClientHeaders();

            if (headers == null)
                return this;

            this.ClientIP = headers.ClientIP;
            this.ClientAgent = headers.ClientAgent;
            this.RequestedURL = req.Url;

            return this;
        }

        /// <summary>Gets the Autodesk Login URL append with the encoded state values </summary>
        /// <param name="state">An OAuth State object to remember various things including scope and App ID</param>
        public string Login( OAuthState state )
        {
            this.Logger.LogInformation($"Request for login");

            // get the base login URL
            StringBuilder sb = new StringBuilder( this.AuthManager.Login( this.Config.CallbackURL ) );

            // add the encoded state
            sb.AppendFormat( "&state={0}", state.Encode() );

            // modify the scopes
            //this.AuthManager.FormatScopes( scopes );

            return sb.ToString();
        }

        /// <summary>Returns the Autodesk Logout URL to the browser to force a logout</summary>
        public string Logout()
        {
            //this.Log( $"Request for logout", LogLevel.Info, "Security" );

            return this.AuthManager.Logout;
        }

        /// <summary>Using the login code gets the users final JWT</summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<TokenResponse> GetToken( string code, OAuthState state = null )
        {
            if (String.IsNullOrWhiteSpace(code))
                return null;

            // get the token from the ID Manager
            Auth0Token results = await this.AuthManager.GetToken( code, this.Config.CallbackURL );

            // validate the reponse
            if ( results == null || String.IsNullOrWhiteSpace( results.Access ) )
                return null;

            // now get the user info
            Auth0User user = await this.AuthManager.GetUserInfo( results.Access );

            // at this point we need to store the client's IP, ID and all the response token details
            //IClientLoginRepository repo = ClientLoginFactory.Make( this.Config.ConnectionString );

            // make a token
            TokenResponse token = new TokenResponse
            {
                Token = results.Access,
                UserID = user?.Email,
                RefreshToken = results.Refresh,
                Expires = results.ExpiresOn
            };

            return token;

            //return (APIResultType.InvalidAppID, token);
        }

        /// <summary>Just returns a simple signature string for testing</summary>
        /// <returns></returns>
        public string Ping(string msg)
        {
            this.Logger.LogInformation($"Pinged = {msg}");

            // default values
            string version = GetVersion();

            // create the signature
            return $"Service : {this.GetType().FullName}; Version : {version}; Env : {this.Config.EnvironmentType}; Message : {msg}";
        }

        /// <summary>A test stub for now where you might call a backend data repo</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetSomeBackendData(int id) => id;

        /// <summary>Gets the version of THIS assembly</summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var vers = name.Version;

            return vers.ToString();
        }
    }


}

