using System;
using System.Data;
using JapeCore;
using JapeHttp;

namespace JapeWeb
{
    public partial class DatabaseApi
    {
        private T MongoRequest<T>(string command, JsonData data, Func<ApiResponse, T> read)
        {
            JsonData mongoData = new(data)
            {
                { "index", MongoIndex },
                { "command", command }
            };

            ApiResponse response = Request(mongoData);

            try
            {
                return read.Invoke(response);
            }
            catch
            {
                throw new DataException($"Mongo command failed: {command}");
            }
        }

        public JsonData MongoGet(string database, string collection, string id)
        {
            return MongoRequest("get", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id }
            }, response => response.ReadJson());
        }

        public JsonData MongoGetWhere(string database, string collection, string field, string value)
        {
            return MongoRequest("get-where", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "field", field },
                { "value", value }
            }, response => response.ReadJson());
        }

        /// <summary>
        /// Inserts a data into a collection.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <param name="collection">The collection name.</param>
        /// <param name="data">The data to insert.</param>
        /// <returns>Id for the inserted data.</returns>
        public string MongoInsert(string database, string collection, JsonData data)
        {
            return MongoRequest("insert", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "data", data }
            }, response => response.Read());
        }

        public JsonData MongoUpdate(string database, string collection, string id, JsonData data)
        {
            return MongoRequest("update", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id },
                { "data", data }
            }, response => response.ReadJson());
        }

        public JsonData MongoRemove(string database, string collection, string id, string[] data)
        {
            return MongoRequest("remove", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id },
                { "data", data }
            }, response => response.ReadJson());
        }

        public JsonData MongoDelete(string database, string collection, string id)
        {
            return MongoRequest("delete", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id }
            }, response => response.ReadJson());
        }
    }
}
