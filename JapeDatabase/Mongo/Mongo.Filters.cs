using System.Linq.Expressions;
using JapeCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JapeDatabase
{
    public partial class Mongo
    {
        public static class Filters
        {
            public static FilterDefinition<BsonDocument> Id(string id) => Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
        }
    }
}
