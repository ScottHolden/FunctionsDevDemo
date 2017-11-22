using FunctionsDevDemo.Service;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Serilog;
using Unity;
using Unity.Resolution;
using WebJobs.ContextResolver;

namespace FunctionsDevDemo.Functions
{
	public class DefaultResolver : IContextResolver
	{
		private readonly IUnityContainer _container;

		public DefaultResolver()
		{
			_container = new UnityContainer()
				.RegisterType<IDataStorage, MongoDataStorage>()
				.RegisterType<IItemService, ItemService>()
				.RegisterInstance(Logger.BaseLogger)
				.RegisterType<MongoDataStorage.ISettings, ApplicationSettings>();
		}

		public T Resolve<T>(FunctionBindingContext context)
		{
			ILogger scopedLogger = _container.Resolve<ILogger>().ForFunctionBindingContext(context);

			return _container.Resolve<T>(new DependencyOverride<ILogger>(scopedLogger));
		}
	}
}