using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raydreams.API.Example.Model;

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
					// you can use Azure Table Logging if you put in a Data Account Connection string
					//log.AddProvider(new AzureTableLoggerProvider( env.ConnectionString) );
                    log.AddProvider( new NullLoggerProvider() );
                })
				.ConfigureServices( (ctx, services) => {
                    services.AddSingleton<EnvironmentSettings>(env);
                    services.AddScoped<IDataGateway, DataGateway>();
                } )
				.Build();

			host.Run();
		}
	}
}