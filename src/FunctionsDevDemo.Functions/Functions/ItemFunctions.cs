using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FunctionsDevDemo.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using WebJobs.ContextResolver;

namespace FunctionsDevDemo.Functions
{
	[ContextResolver(typeof(DefaultResolver))]
	public class ItemFunctions
	{
		[FunctionName(nameof(GetItem))]
		public static async Task<ItemDetails> GetItem(
			[HttpTrigger(AuthorizationLevel.Anonymous, new string[] { "GET" }, Route = "item/{id}")] HttpRequestMessage req,
			string id,
			[Resolve] IItemService itemService)
		{
			ItemDetails item = await itemService.GetAsync(id);

			return item;
		}

		[FunctionName(nameof(UpdateItem))]
		public static async Task<HttpResponseMessage> UpdateItem(
			[HttpTrigger(AuthorizationLevel.Anonymous, new string[] { "PUT" }, Route = "item/{id}")] ItemDetails item,
			string id,
			[Resolve] IItemService itemService)
		{
			await itemService.UpdateAsync(id, item);

			return new HttpResponseMessage(HttpStatusCode.OK);
		}

		[FunctionName(nameof(NewItem))]
		public static async Task<HttpResponseMessage> NewItem(
			[HttpTrigger(AuthorizationLevel.Anonymous, new string[] { "POST" }, Route = "item")] ItemDetails item,
			[Resolve] IItemService itemService)
		{
			string id = await itemService.InsertAsync(item);

			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(id)
			};
		}
	}
}