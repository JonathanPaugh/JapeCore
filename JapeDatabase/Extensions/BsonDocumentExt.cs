using JapeCore;
using MongoDB.Bson;

namespace JapeDatabase
{
    public static class BsonDocumentExt
    {
        public static string GetId(this BsonDocument document)
        {
            return document["_id"].ToJson(Bson.CanonicalJson);
        }

        /// <summary>
        /// Converts a bson document to a json data.
        /// 
        /// Converts Bson objects to Json objects. 
        /// Bson: "object" : Object("value"),
        /// Json: "object" : { "$key" : "value" }
        ///
        /// </summary>
        /// <param name="document">The bson document.</param>
        /// <para></para>
        /// <returns></returns>
        public static JsonData ToJsonData(this BsonDocument document)
        {
            string json = document.ToJson(typeof(BsonDocument), Bson.CanonicalJson);
            return new JsonData(json);
        }
    }
}
