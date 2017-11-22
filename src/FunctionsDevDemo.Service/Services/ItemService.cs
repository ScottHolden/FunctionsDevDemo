using System;
using System.Threading.Tasks;

namespace FunctionsDevDemo.Service
{
	public sealed class ItemService : IItemService
	{
		private readonly IDataStorage _dataStorage;

		public ItemService(IDataStorage dataStorage)
		{
			_dataStorage = dataStorage;
		}

		public Task<ItemDetails> GetAsync(string id) => _dataStorage.GetAsync<ItemDetails>(id);

		public Task<string> InsertAsync(ItemDetails value)
		{
			ValidateItemDeatils(value);

			return _dataStorage.InsertAsync(value);
		}

		public Task UpdateAsync(string id, ItemDetails value)
		{
			ValidateItemDeatils(value);

			return _dataStorage.UpdateAsync(id, value);
		}

		private void ValidateItemDeatils(ItemDetails value)
		{
			if (value.Qty < 0)
			{
				throw new Exception("Qty must be > 0");
			}
		}
	}
}