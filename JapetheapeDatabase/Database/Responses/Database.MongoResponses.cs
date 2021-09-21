using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using JapeHttp;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JapeDatabase
{
    public partial class Database
    {
        private Dictionary<string, Action<HttpRequest, HttpResponse, Dictionary<string, JsonElement>>> mongoResponses;
        private void MongoResponses()
        {
            mongoResponses = new Dictionary<string, Action<HttpRequest, HttpResponse, Dictionary<string, JsonElement>>>
            {
                { "Get", ResponseMongoGet },
                { "Insert", ResponseMongoInsert },
                { "Update", ResponseMongoUpdate },
                { "Remove", ResponseMongoRemove },
                { "Delete", ResponseMongoDelete },
            };
        }

        public async void ResponseMongoGet(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Find Request");

            IMongoDatabase database = mongo.GetDatabase(data["store"].GetString());

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data["collection"].GetString());

            BsonDocument document = await collection.Find(Mongo.Filters.Id(data["key"].GetString())).FirstOrDefaultAsync();

            response.StatusCode = 200;
            await response.Write(document.ToJson());
            await response.CompleteAsync();
        }

        public async void ResponseMongoInsert(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Insert Request");

            IMongoDatabase database = mongo.GetDatabase(data["store"].GetString());

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data["collection"].GetString());

            BsonDocument document = BsonDocument.Parse(data["data"].GetString());

            await collection.InsertOneAsync(document);

            response.StatusCode = 200;
            await response.Write(document.GetId());
            await response.CompleteAsync();
        }

        public async void ResponseMongoUpdate(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Update Request");

            IMongoDatabase database = mongo.GetDatabase(data["store"].GetString());

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data["collection"].GetString());

            BsonDocument elements = BsonDocument.Parse(data["data"].GetString());

            List<UpdateDefinition<BsonDocument>> updates = new List<UpdateDefinition<BsonDocument>>();

            foreach (BsonElement element in elements)
            {
                updates.Add(Builders<BsonDocument>.Update.Set(element.Name, element.Value));
            }

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Combine(updates);

            FindOneAndUpdateOptions<BsonDocument> options = new FindOneAndUpdateOptions<BsonDocument>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            BsonDocument document = await collection.FindOneAndUpdateAsync(Mongo.Filters.Id(data["key"].GetString()), update, options);

            response.StatusCode = 200;
            await response.Write(document.ToJson());
            await response.CompleteAsync();
        }

        public async void ResponseMongoRemove(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Update Request");

            IMongoDatabase database = mongo.GetDatabase(data["store"].GetString());

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data["collection"].GetString());

            List<UpdateDefinition<BsonDocument>> updates = new List<UpdateDefinition<BsonDocument>>();

            foreach (JsonElement element in data["data"].EnumerateArray())
            {
                updates.Add(Builders<BsonDocument>.Update.Unset(element.ToString()));
            }

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Combine(updates);

            FindOneAndUpdateOptions<BsonDocument> options = new FindOneAndUpdateOptions<BsonDocument>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            BsonDocument document = await collection.FindOneAndUpdateAsync(Mongo.Filters.Id(data["key"].GetString()), update, options);

            response.StatusCode = 200;
            await response.Write(document.ToJson());
            await response.CompleteAsync();
        }

        public async void ResponseMongoDelete(HttpRequest request, HttpResponse response, Dictionary<string, JsonElement> data)
        {
            Log.Write("Delete Request");

            IMongoDatabase database = mongo.GetDatabase(data["store"].GetString());

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(data["collection"].GetString());

            BsonDocument document = await collection.FindOneAndDeleteAsync(Mongo.Filters.Id(data["key"].GetString()));

            response.StatusCode = 200;
            await response.Write(document.ToJson());
            await response.CompleteAsync();
        }
    }
}
