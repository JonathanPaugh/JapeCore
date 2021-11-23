using MongoDB.Bson;

namespace JapeDatabase
{
    public static class BsonDocumentExt
    {
        public static string GetId(this BsonDocument document)
        {
            return document["_id"].ToString();
        }
    }
}
