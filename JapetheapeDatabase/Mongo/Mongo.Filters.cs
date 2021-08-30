using MongoDB.Bson;
using MongoDB.Driver;

namespace JapeDatabase
{
    public partial class Mongo
    {
        public static class Filters
        {
            public static FilterDefinition<BsonDocument> Id(string id)
            {
                return Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
            }
        }
    }
}
