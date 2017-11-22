using System.Threading.Tasks;

namespace FunctionsDevDemo.Service
{
	public interface IItemService
	{
		Task<ItemDetails> GetAsync(string id);

		Task UpdateAsync(string id, ItemDetails value);

		Task<string> InsertAsync(ItemDetails value);
	}
}