using System;

namespace Raydreams.API.Example.Model
{
    /// <summary>Types of Physical Data Stores</summary>
    public enum PhysicalDataSource
    {
        Unknown = 0,
        AzureTables = 1,
        AzureInsights = 2,
        MongoDB = 3,
        Redis = 4,
        SQLServer = 5
    }

    /// <summary></summary>
    public static class DataSourceExtensions
    {
        /// <summary>Determines what kind of data source being used based on the connection string</summary>
        /// <param name="connStr">The connection srtring to the physical data store</param>
        /// <returns></returns>
        public static PhysicalDataSource SniffDataSource( this string connStr )
        {
            if ( String.IsNullOrWhiteSpace( connStr ) )
                return PhysicalDataSource.Unknown;

            connStr = connStr.Trim().ToLowerInvariant();

            if ( connStr.StartsWith( "mongodb" ) )
                return PhysicalDataSource.MongoDB;

            return PhysicalDataSource.AzureTables;
        }
    }

}

