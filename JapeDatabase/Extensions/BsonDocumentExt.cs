using System.Text.RegularExpressions;
using JapeCore;
using MongoDB.Bson;

namespace JapeDatabase
{
    public static class BsonDocumentExt
    {
        public static string GetId(this BsonDocument document)
        {
            return document["_id"].ToString();
        }

        public static string ToJson(this BsonDocument document)
        {
            string data = document.ToJson(typeof(BsonDocument));
            data = Regex.Replace(data, @"ObjectId\((.*?)\)", match => match.Groups[1].Value);
            return data;
        }
    }
}
