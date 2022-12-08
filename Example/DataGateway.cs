using System;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Raydreams.API.Example.Extensions;
using Raydreams.API.Example.Model;

namespace Raydreams.API.Example
{
    /// <summary></summary>
	public interface IDataGateway
	{
        IDataGateway AddHeaders(HttpRequestData req);

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

        public DataGateway( EnvironmentSettings settings, ILogger<DataGateway> logger )
		{
            this.Config = settings;
            this.Logger = logger;
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

        /// <summary>The default logger</summary>
        public ILogger<DataGateway> Logger { get; set; }

        /// <summary>Delegate Function for converting Markdown to HTML. Input string is markdown and return is HTML.</summary>
        /// <returns>Converted HTML</returns>
        /// <remarks>Supply your own Markdown conversion routine</remarks>
        //public Func<string, string> ConvertMarkdown { get; set; }

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

        /// <summary>Gets the version of THIS assembly</summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var vers = name.Version;

            return vers.ToString();
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

        public int GetSomeBackendData(int id) => id;
    }
}

