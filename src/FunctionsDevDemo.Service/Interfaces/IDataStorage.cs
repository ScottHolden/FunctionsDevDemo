using System.Threading.Tasks;

namespace FunctionsDevDemo.Service
{
	public interface IDataStorage
	{
		Task<T> GetAsync<T>(string id);

		Task UpdateAsync<T>(string id, T value);

		Task<string> InsertAsync<T>(T value);
	}
}