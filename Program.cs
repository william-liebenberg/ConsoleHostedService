using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ConsoleHostedService
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await Host.CreateDefaultBuilder(args)
				.ConfigureLogging(logging =>
				{
					// Add any 3rd party loggers like NLog or Serilog
				})
				.ConfigureAppConfiguration((hostContext, config) =>
				{
					// Add User Secrets for local development
					if (hostContext.HostingEnvironment.IsDevelopment())
					{
						config.AddUserSecrets(typeof(Program).Assembly);
					}
				})
				.ConfigureServices((hostContext, services) =>
				{
					// Add the console hosted service
					services.AddHostedService<ConsoleHostedService>();

					// Add any other application services
					services.AddSingleton<IDataService, DataService>();

					// Register any AppSettings that are injected via IOptions<>
					services.Configure<DataSettings>(hostContext.Configuration.GetSection("Data"));
				})
				.RunConsoleAsync();
		}
	}
}
