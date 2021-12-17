using System;
using System.Data;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;

namespace JapeWeb
{
    public partial class DatabaseApi
    {
        private async Task<T> MongoRequest<T>(string command, JsonData data, Func<ApiResponse, T> read)
        {
            JsonData mongoData = new(data)
            {
                { "index", MongoIndex },
                { "command", command }
            };

            ApiResponse response = await Request(mongoData);

            try
            {
                return read.Invoke(response);
            }
            catch
            {
                throw new DataException($"Mongo command failed: {command}");
            }
        }

        public async Task<JsonData> MongoGet(string database, string collection, string id)
        {
            return await MongoRequest("get", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id }
            }, response => response.Json);
        }

        public async Task<JsonData> MongoGetWhere(string database, string collection, string field, string value)
        {
            return await MongoRequest("get-where", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "field", field },
                { "value", value }
            }, response => response.Json);
        }

        /// <summary>
        /// Inserts a data into a collection.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <param name="collection">The collection name.</param>
        /// <param name="data">The data to insert.</param>
        /// <returns>Id for the inserted data.</returns>
        public async Task<string> MongoInsert(string database, string collection, JsonData data)
        {
            return await MongoRequest("insert", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "data", data }
            }, response => response.Data);
        }

        public async Task<JsonData> MongoUpdate(string database, string collection, string id, JsonData data)
        {
            return await MongoRequest("update", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id },
                { "data", data }
            }, response => response.Json);
        }

        public async Task<JsonData> MongoRemove(string database, string collection, string id, string[] data)
        {
            return await MongoRequest("remove", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id },
                { "data", data }
            }, response => response.Json);
        }

        public async Task<JsonData> MongoDelete(string database, string collection, string id)
        {
            return await MongoRequest("delete", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "id", id }
            }, response => response.Json);
        }
    }
}
