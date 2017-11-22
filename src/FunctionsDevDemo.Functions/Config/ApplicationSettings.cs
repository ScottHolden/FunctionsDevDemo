using System.Configuration;
using FunctionsDevDemo.Service;

namespace FunctionsDevDemo.Functions
{
	internal class ApplicationSettings : MongoDataStorage.ISettings
	{
		// This is kinda ugly, and it's better when you replace it with
		//  CastleProxy/Unity.Interception and hook these up on the fly :)
		string MongoDataStorage.ISettings.ConnectionString => GetRequiredSetting(nameof(MongoDataStorage), nameof(MongoDataStorage.ISettings.ConnectionString));

		string MongoDataStorage.ISettings.DatabaseName => GetRequiredSetting(nameof(MongoDataStorage), nameof(MongoDataStorage.ISettings.DatabaseName));

		private static string GetRequiredSetting(string service, string name)
		{
			string settingName = $"{service}.{name}";

			string value = ConfigurationManager.AppSettings[settingName];

			if (string.IsNullOrEmpty(value))
			{
				throw new ConfigurationErrorsException("Couldn't find required setting " + settingName);
			}

			return value;
		}
	}
}