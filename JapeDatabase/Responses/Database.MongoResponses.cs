using System.Collections.Generic;
using System.Threading.Tasks;
using JapeHttp;
using JapeCore;
using JapeService.Responder;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JapeDatabase
{
    public partial class Database
    {
        private ResponseBank<string> MongoResponses => new()
        {
            { "get", ResponseMongoGet },
            { "get-where", ResponseMongoGetWhere },
            { "insert", ResponseMongoInsert },
            { "update", ResponseMongoUpdate },
            { "remove", ResponseMongoRemove },
            { "delete", ResponseMongoDelete },
        };

        public async Task<Request.Result> ResponseMongoGet(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Get Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument document = await collection.Find(Mongo.Filters.Id(data.GetString("id"))).FirstOrDefaultAsync();

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }

        public async Task<Request.Result> ResponseMongoGetWhere(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Get Where Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument document = await collection.Find(Builders<BsonDocument>.Filter.Eq(data.GetString("field"), data.GetString("value"))).FirstOrDefaultAsync();

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }

        public async Task<Request.Result> ResponseMongoInsert(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Insert Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument document = BsonDocument.Parse(data.GetString("data"));

            await collection.InsertOneAsync(document);

            return await transfer.Complete(Status.SuccessCode.Ok, document.GetId());
        }

        public async Task<Request.Result> ResponseMongoUpdate(Responder<string>.Transfer transfer, JsonData data, object[] args)
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

            BsonDocument document = await collection.FindOneAndUpdateAsync(Mongo.Filters.Id(data.GetString("id")), update, options);

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }

        public async Task<Request.Result> ResponseMongoRemove(Responder<string>.Transfer transfer, JsonData data, object[] args)
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

            BsonDocument document = await collection.FindOneAndUpdateAsync(Mongo.Filters.Id(data.GetString("id")), update, options);

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }

        public async Task<Request.Result> ResponseMongoDelete(Responder<string>.Transfer transfer, JsonData data, object[] args)
        {
            Log.Write("Delete Request");

            IMongoDatabase database = mongo.GetDatabase(data.GetString("store"));

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data.GetString("collection"));

            BsonDocument document = await collection.FindOneAndDeleteAsync(Mongo.Filters.Id(data.GetString("id")));

            return await transfer.Complete(Status.SuccessCode.Ok, document.ToJson());
        }
    }
}
