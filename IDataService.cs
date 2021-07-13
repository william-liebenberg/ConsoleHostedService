using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleHostedService
{
	public interface IDataService
	{
		Task<IReadOnlyList<string>> QueryData();
	}
}
