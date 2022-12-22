using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
					if ( String.IsNullOrWhiteSpace(env.ConnectionString) )
                        log.AddProvider( new NullLoggerProvider() );
					else
						log.AddProvider(new AzureTableLoggerProvider( env.ConnectionString) );
                })
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