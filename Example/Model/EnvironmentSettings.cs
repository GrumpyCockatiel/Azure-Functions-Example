using System;
using Raydreams.API.Example.Extensions;

namespace Raydreams.API.Example.Model
{
    /// <summary>Encapsulates the settings are various environments.</summary>
    public class EnvironmentSettings
    {
        // These match to the keys in the Azure Function Configuation section or local.settings
        #region [ Config Keys ]

        /// <summary></summary>
        public static readonly string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";

        /// <summary></summary>
        public static readonly string ConnectionStringKey = "connStr";

        #endregion [ Config Keys ]

        /// <summary>Main constructor loads Config settings</summary>
        public EnvironmentSettings()
        {
            // load client details
            this.ConnectionString = Environment.GetEnvironmentVariable(ConnectionStringKey);
        }

        /// <summary>Gets environment settings from a string based on the enum value</summary>
        public static EnvironmentSettings GetSettings(string type)
        {
            EnvironmentType env = type.GetEnumValue<EnvironmentType>(EnvironmentType.Unknown);

            EnvironmentSettings set = GetSettings(env);

            set.EnvironmentKeyValue = type;
            return set;
        }

        /// <summary>Gets environment settings from an enum value</summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EnvironmentSettings GetSettings(EnvironmentType type)
        {
            if (type == EnvironmentType.Production)
                return PROD;
            else if (type == EnvironmentType.Development)
                return DEV;
            else if (type == EnvironmentType.Local)
                return LOCAL;
            //else if ( type == EnvironmentType.Testing )
            //return TEST;
            //else if ( type == EnvironmentType.Staging )
            //return STG;
            else
                return DEV;
        }

        #region [ Properties ]

        /// <summary>The enumerated environment type</summary>
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>The actual key used to load the environment</summary>
        public string EnvironmentKeyValue { get; set; }

        /// <summary>The connection string to the Storage Account</summary>
        public string ConnectionString { get; set; }

        #endregion [ Properties ]

        /// <summary>Unknown environment settings</summary>
        public static EnvironmentSettings LOCAL
        {
            get
            {
                return new EnvironmentSettings()
                {
                    EnvironmentType = EnvironmentType.Local
                };
            }
        }

        /// <summary>DEV environment settings</summary>
        public static EnvironmentSettings DEV
        {
            get
            {
                return new EnvironmentSettings()
                {
                    EnvironmentType = EnvironmentType.Development
                };
            }
        }

        /// <summary>PROD environment settings</summary>
        public static EnvironmentSettings PROD
        {
            get
            {
                return new EnvironmentSettings()
                {
                    EnvironmentType = EnvironmentType.Production
                };

            }
        }
    }
}

