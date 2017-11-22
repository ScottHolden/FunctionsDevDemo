using System;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;

namespace FunctionsDevDemo.Functions
{
	internal static class Logger
	{
		public static ILogger BaseLogger => _lazyLogger.Value;

		private static Lazy<ILogger> _lazyLogger = new Lazy<ILogger>(BuildLogger);

		private static ILogger BuildLogger() => new LoggerConfiguration()
													.Enrich.WithMachineName()
													.CreateLogger();

		public static ILogger ForFunctionBindingContext(this ILogger baseLogger, FunctionBindingContext context)
			=> new LoggerConfiguration()
					.WriteTo.Logger(baseLogger)
					.WriteTo.TraceWriter(context.Trace)
					.Enrich.WithProperty(nameof(context.FunctionInstanceId), context.FunctionInstanceId)
					.CreateLogger();
	}
}