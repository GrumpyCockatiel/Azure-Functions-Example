using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raydreams.API.Example.Logging;

namespace Raydreams.API.Example
{
	/// <summary></summary>
	public class Program
	{
		public static void Main()
		{
			// read Configuration Settings
            var env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable(EnvironmentSettings.EnvironmentKey) );

            IHost host = new HostBuilder()
				.ConfigureFunctionsWorkerDefaults()
				.ConfigureLogging(log =>
				{
					log.ClearProviders();
                    // you can use Azure Table Logging or other custom logger if you put in a Connection string
                    // you can send the connection string to a Factory Class here

                    var logBuilder = LogFactory.MakeLogger( env.LogConnectionString, "API Example", LogLevel.Trace );
                    log.AddProvider( logBuilder );

                    //if ( String.IsNullOrWhiteSpace(env.LogConnectionString) )
                        //log.AddProvider( new NullLoggerProvider() );
					//else
						//log.AddProvider( new AzureTableLoggerProvider(env.LogConnectionString, "API Example ") );
						//log.AddProvider( new MongoLoggerProvider( env.LogConnectionString, env.DatabaseName, "API Example " ) );
                } )
				.ConfigureServices( (ctx, services) => {
					// add the Settings as a singleton
                    services.AddSingleton<EnvironmentSettings>(env);
                    services.AddScoped<IForecastService, NWSForecastService>( s => new NWSForecastService { Station = "HGX", Grid = "65,97" } );
                    services.AddScoped<IDataGateway, DataGateway>();
                } )
				.Build();

			host.Run();
		}
	}
}