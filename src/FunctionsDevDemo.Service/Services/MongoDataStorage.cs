using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FunctionsDevDemo.Service
{
	public sealed class MongoDataStorage : IDataStorage
	{
		private readonly IMongoDatabase _database;
		private const string IDField = "_id";

		public interface ISettings
		{
			string ConnectionString { get; }

			string DatabaseName { get; }
		}

		public MongoDataStorage(ISettings settings)
		{
			MongoClient mongoClient = new MongoClient(settings.ConnectionString);

			_database = mongoClient.GetDatabase(settings.DatabaseName);
		}

		public async Task<T> GetAsync<T>(string id)
		{
			IMongoCollection<T> collection = GetCollection<T>();

			FilterDefinition<T> filter = Builders<T>.Filter.Eq(IDField, new ObjectId(id));

			ProjectionDefinition<T> projection = Builders<T>.Projection.Exclude(IDField);

			return await collection.Find(filter).Project<T>(projection).FirstOrDefaultAsync();
		}

		public async Task UpdateAsync<T>(string id, T value)
		{
			IMongoCollection<T> collection = GetCollection<T>();

			FilterDefinition<T> filter = Builders<T>.Filter.Eq(IDField, new ObjectId(id));

			await collection.ReplaceOneAsync(filter, value);
		}

		public async Task<string> InsertAsync<T>(T value)
		{
			IMongoCollection<BsonDocument> collection = GetBsonDocumentCollection<T>();

			BsonDocument document = value.ToBsonDocument();

			await collection.InsertOneAsync(document);

			return document[IDField].AsObjectId.ToString();
		}

		private IMongoCollection<T> GetCollection<T>() => _database.GetCollection<T>(GetCollectionName<T>());

		private IMongoCollection<BsonDocument> GetBsonDocumentCollection<T>() => _database.GetCollection<BsonDocument>(GetCollectionName<T>());

		private string GetCollectionName<T>() => typeof(T).Name;
	}
}