using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

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
