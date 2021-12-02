using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using JapeHttp;
using JapeCore;
using JapeService;
using JapeService.Responder;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace JapeDatabase
{
    public partial class Database
    {
        private ResponseBank<string> MongoResponses => new()
        {
            { "get", ResponseMongoGet },
            { "insert", ResponseMongoInsert },
            { "update", ResponseMongoUpdate },
            { "remove", ResponseMongoRemove },
            { "delete", ResponseMongoDelete },
        };

        public async Task<Resolution> ResponseMongoGet(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Get Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument document = await collection.Find(Mongo.Filters.Id(data.GetString("key"))).FirstOrDefaultAsync();

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }

        public async Task<Resolution> ResponseMongoInsert(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Insert Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument document = BsonDocument.Parse(data.GetString("data"));

            await collection.InsertOneAsync(document);

            return await transfer.Complete(Status.SuccessCode.Ok, document.GetId());
        }

        public async Task<Resolution> ResponseMongoUpdate(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Update Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument elements = BsonDocument.Parse(data.GetString("data"));

            List<UpdateDefinition<BsonDocument>> updates = new();

            foreach (BsonElement element in elements)
            {
                updates.Add(Builders<BsonDocument>.Update.Set(element.Name, element.Value));
            }

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Combine(updates);

            FindOneAndUpdateOptions<BsonDocument> options = new()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            BsonDocument document = await collection.FindOneAndUpdateAsync(Mongo.Filters.Id(data.GetString("key")), update, options);

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }

        public async Task<Resolution> ResponseMongoRemove(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Update Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            List<UpdateDefinition<BsonDocument>> updates = new();

            foreach (string element in data.GetStringArray("data"))
            {
                updates.Add(Builders<BsonDocument>.Update.Unset(element));
            }

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Combine(updates);

            FindOneAndUpdateOptions<BsonDocument> options = new()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            BsonDocument document = await collection.FindOneAndUpdateAsync(Mongo.Filters.Id(data.GetString("key")), update, options);

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }

        public async Task<Resolution> ResponseMongoDelete(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Delete Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument document = await collection.FindOneAndDeleteAsync(Mongo.Filters.Id(data.GetString("key")));

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }
    }
}
