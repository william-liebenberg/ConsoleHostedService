using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleHostedService
{
	public class DataService : IDataService
	{
		private readonly IOptions<DataSettings> _settings;

		public DataService(IOptions<DataSettings> settings)
		{
			_settings = settings;
		}

		public Task<IReadOnlyList<string>> QueryData()
		{
			string[] words = new[] { "apple", "banana", "cantilope", "durian" };

			if (_settings.Value.Transform)
			{
				for (int i = 0; i < words.Length; i++)
				{
					words[i] = words[i].ToUpper();
				}
			}

			return Task.FromResult<IReadOnlyList<string>>(words);
		}
	}
}
