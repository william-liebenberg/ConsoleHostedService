using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleHostedService
{
	public class ConsoleHostedService : IHostedService
	{
		private readonly ILogger _logger;
		private readonly IHostApplicationLifetime _appLifetime;
		private readonly IDataService _dataService;

		private int? _exitCode;

		public ConsoleHostedService(
			ILogger<ConsoleHostedService> logger,
			IHostApplicationLifetime appLifetime,
			IDataService dataService)
		{
			_logger = logger;
			_appLifetime = appLifetime;
			_dataService = dataService;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

			_appLifetime.ApplicationStarted.Register(() =>
			{
				Task.Run(async () =>
				{
					try
					{
						FigletText banner = new("Hosted Service - Console App");
						AnsiConsole.Render(banner
							.Centered()
							.Color(Color.Yellow));

						Table table = new();
						table.AddColumn("[green]Id[/]");
						table.AddColumn(new TableColumn("[red]Value[/]").Centered());

						BarChart barChart = new BarChart()
							.Width(60)
							.Label("[green bold underline]Query Results[/]")
							.CenterLabel();

						Color[] barColors = new[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Purple };

						IReadOnlyList<string> data = await _dataService.QueryData();
						for (int i = 0; i < data.Count; i++)
						{
							table.AddRow($"[blue]{i}[/]", data[i]);
							barChart.AddItem(data[i], data[i]?.Length ?? 0, barColors[i % barColors.Length]);
						}

						AnsiConsole.Render(table);
						AnsiConsole.Render(barChart);

						_exitCode = 0;
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Unhandled exception!");
						_exitCode = 1;
					}
					finally
					{
						// Stop the application once the work is done
						_appLifetime.StopApplication();
					}
				});
			});

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogDebug($"Exiting with return code: {_exitCode}");

			// Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
			Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
			return Task.CompletedTask;
		}
	}
}
