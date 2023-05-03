using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Raydreams.API.Example.Logging
{
    /// <summary></summary>
    public class LogFactory
    {
        /// <summary>Make the right logger based on the connection string</summary>
        /// <param name="connStr"></param>
        /// <param name="lvl"></param>
        /// <returns></returns>
        /// <remarks>
        /// DB Name, Table Name and Source need to be settable possibly extracted from the conn string
        /// </remarks>
        public static ILoggerProvider MakeLogger( string connStr, string source, LogLevel lvl = LogLevel.Information )
        {
            ILoggerProvider logger = null;

            PhysicalDataSource dataSource = connStr.SniffDataSource();

            switch ( dataSource )
            {
                case PhysicalDataSource.AzureTables:
                    logger = new AzureTableLoggerProvider( connStr );
                    break;
                case PhysicalDataSource.MongoDB:
                    source = ( !String.IsNullOrWhiteSpace( source ) ) ? source.Trim() : $"unknown";
                    logger = new MongoLoggerProvider( connStr, connStr.ParseMongoConnStr(), source );
                    break;
                default:
                    logger = NullLoggerProvider.Instance;
                    break;
            }

            return logger;
        }
    }
}

