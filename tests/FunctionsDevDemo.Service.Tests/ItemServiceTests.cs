using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace FunctionsDevDemo.Service.Tests
{
	public class ItemServiceTests
	{
		[Fact]
		public void InsertAsync_InvalidQty_NoException()
		{
			Mock<IDataStorage> storage = new Mock<IDataStorage>(MockBehavior.Strict);

			ItemService itemService = new ItemService(storage.Object);

			Assert.ThrowsAsync<Exception>(async () =>
				await itemService.InsertAsync(new ItemDetails
				{
					Qty = -7,
					Name = "This Should Break"
				})
			);
		}

		[Fact]
		public async Task InsertAsync_ValidQty_NoException()
		{
			Mock<IDataStorage> storage = new Mock<IDataStorage>(MockBehavior.Strict);

			string expectedID = "aaaa";

			storage.Setup(x => x.InsertAsync(It.IsAny<ItemDetails>())).ReturnsAsync(expectedID);

			ItemService itemService = new ItemService(storage.Object);

			string id = await itemService.InsertAsync(new ItemDetails
			{
				Qty = 7,
				Name = "This Should Be Fine"
			});

			Assert.Equal(expectedID, id);
		}
	}
}