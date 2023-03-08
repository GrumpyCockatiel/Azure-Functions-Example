using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Raydreams.API.Example
{
    /// <summary>Provider for logging to Azure Tables</summary>
    public sealed class MongoLoggerProvider : ILoggerProvider
    {
        private string _connStr = null;

        private string _source = null;

        private string _db = null;

        // assumes the table name is 'Logs'
        public MongoLoggerProvider( string connStr, string db, string source = null )
        {
            this._connStr = connStr;
			this._db = db;
			this._source = source;
        }

        /// <summary>ILoggerProvider</summary>
		/// <remarks>Hardcoded to a Logs Table Name</remarks>
        public ILogger CreateLogger( string categoryName )
        {
            return new MongoLogger( this._connStr, this._db, "Logs" ) { Source = this._source, Category = categoryName };
        }

        public void Dispose()
        {
            return;
        }
    }

    /// <summary>Logs to a Mongo DB Collection usually named Logs</summary>
    /// <remarks>This class is pretty old and could probably use a review and cleanup</remarks>
    public class MongoLogger : MongoDataManager<LogRecord, long>, ILogger
	{
        #region [Fields]

        private LogLevel _level = LogLevel.Trace;

        private string _table = "Logs";

		private string _src = null;

		#endregion [Fields]

		#region [Constructor]

		/// <summary>Constructor</summary>
		/// <param name="connStr"></param>
		/// <param name="db"></param>
		/// <param name="table"></param>
		/// <param name="src"></param>
		public MongoLogger( string connStr, string db, string table, string src = null ) : base( connStr, db )
		{
			this.TableName = table;
			this.Source = src;
		}

		#endregion [Constructor]

		#region [Properties]

		/// <summary>The maximum number of logs to return</summary>
		public int Max { get; set; } = 200;

		/// <summary>The name of the logging table or collection</summary>
		public string TableName
		{
			get { return this._table; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._table = value.Trim();
			}
		}

        /// <summary>The minimum level to log which defaults to All</summary>
        public LogLevel Level
        {
            get { return this._level; }
            set { this._level = value; }
        }

        /// <summary>The logging source. Who is doing the logging.</summary>
        public string Source
        {
            get { return this._src; }
            set
            {
                if ( value != null )
                    this._src = value.Trim();
            }
        }

        /// <summary>The Log Category</summary>
        public string Category { get; set; }

        #endregion [Properties]

        #region [ Base Methods ]

        /// <summary>Set the minimum level to log</summary>
        /// <param name="lvl"></param>
        /// <returns></returns>
        public MongoLogger SetLevel( LogLevel lvl )
		{
			this.Level = lvl;
			return this;
		}

		/// <summary>Sample method that just gets all the logs</summary>
		/// <remarks>Generally do not want to use this</remarks>
		public List<LogRecord> List()
		{
			return base.GetAll( this.TableName );
		}

		/// <summary>Gets only the top N logs sorted descending by timestamp</summary>
		/// <returns></returns>
		public List<LogRecord> ListTop( int top = 100 )
		{
			if ( top > this.Max )
				top = this.Max;

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			List<LogRecord> results = collection.Find( FilterDefinition<LogRecord>.Empty ).Sort( "{timestamp: -1}" ).Limit( top ).ToList();

			return ( results != null && results.Count > 0 ) ? results : new List<LogRecord>();
		}

		/// <summary>Gets logs within a date range</summary>
		/// <returns></returns>
		public List<LogRecord> ListByRange( DateTimeOffset begin, DateTimeOffset end )
		{
			if ( begin > end )
				return new List<LogRecord>();

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			return collection.Find<LogRecord>( t => t.Timestamp >= begin && t.Timestamp < end ).ToList();
		}

		/// <summary>Deletes any logs older than the specified number of days</summary>
		/// <param name="days">Number of days</param>
		/// <returns>Records removed</returns>
		public async Task<long> PurgeAfter( int days = 90 )
		{
			if ( days < 0 )
				return 0;

			DateTime expire = DateTime.UtcNow.Subtract( new TimeSpan( days, 0, 0, 0 ) );

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			DeleteResult results = await collection.DeleteManyAsync<LogRecord>( t => t.Timestamp < expire );

			return ( results.IsAcknowledged ) ? results.DeletedCount : 0;
		}

        #endregion [ Base Methods ]

        #region [ ILogger ]

        public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter )
        {
            var argsDict = state as IReadOnlyList<KeyValuePair<string, object>>;
            var argsArray = argsDict.Take( argsDict.Count - 1 ).Select( a => $"{a.Key}={a.Value}" ).ToArray();

            this.InsertLog( logLevel, this.Category, state.ToString(), argsArray );
        }

        /// <summary>All log levels enabled</summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled( LogLevel logLevel )
        {
            return logLevel >= this.Level;
        }

        public IDisposable BeginScope<TState>( TState state ) => default!;

        #endregion [ ILogger ]

        /// <summary>Base logging method</summary>
        /// <param name="lvl">The std log level as defined by Log4j</param>
        /// <param name="category">An application specific category that can be used for further organization, or routing to differnt locations/</param>
        /// <param name="msg">The actual message to log</param>
        /// <param name="args">any additional data fields to append to the log message. Used for debugging.</param>
        /// <returns></returns>
        protected bool InsertLog( LogLevel lvl, string category, string msg, params object[] args )
		{
			if ( lvl < this.Level )
				return false;

			try
			{
				string src = String.IsNullOrWhiteSpace( this.Source ) ? Assembly.GetExecutingAssembly().FullName : this.Source;

				// convert the args dictionary to a string and add to the end
				//if ( args != null && args.Length > 0 )
				//msg = String.Format( "{0} args={1}", msg, String.Join( ";", args ) );

				if ( String.IsNullOrWhiteSpace( msg ) )
					msg = String.Empty;

				// inserts are fire and forget
				_ = base.Insert( new LogRecord( msg, lvl ) { Category = category, Source = src, Args = args }, this.TableName );
			}
			catch ( System.Exception )
			{
				return false;
			}

			return true;
		}

	}
}
