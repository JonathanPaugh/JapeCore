using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using JapeCore;
using JapeHttp;

namespace JapeDatabase
{
    public partial class DatabaseApi
    {
        public enum MongoQueryResult { Default, Cursor, First }

        private async Task<T> MongoRequest<T>(string command, JsonData data, Func<ApiResponse, Task<T>> read)
        {
            JsonData mongoData = new(data)
            {
                { "index", Database.MongoIndex },
                { "command", command }
            };

            ApiResponse response = await Request(mongoData);

            try
            {
                return await read.Invoke(response);
            }
            catch
            {
                throw new DataException($"Unable to read mongo response: {command}");
            }
        }

        public async Task<JsonData> MongoQuery(string database, JsonData query, MongoQueryResult queryResult = MongoQueryResult.Default)
        {
            return await MongoRequest("query", new JsonData
            {
                { "database", database },
                { "query", query },
            }, async response =>
            {
                JsonData data = await response.ReadJsonAsync();
                switch (queryResult)
                {
                    default: return data;
                    case MongoQueryResult.Cursor: return data.Extract("cursor");
                    case MongoQueryResult.First: return data.Extract("cursor").ExtractArray("firstBatch").First();
                }
            });
        }

        public async Task<JsonData> MongoGet(string database, string collection, string id)
        {
            return await MongoRequest("get", new JsonData
            {
                { "database", database },
                { "collection", collection },
                { "id", id }
            }, async response => await response.ReadJsonAsync());
        }

        public async Task<JsonData> MongoInsert(string database, string collection, JsonData data)
        {
            return await MongoRequest("insert", new JsonData
            {
                { "database", database },
                { "collection", collection },
                { "data", data }
            }, async response => await response.ReadJsonAsync());
        }

        public async Task<JsonData> MongoUpdate(string database, string collection, string id, JsonData data)
        {
            return await MongoRequest("update", new JsonData
            {
                { "database", database },
                { "collection", collection },
                { "id", id },
                { "data", data }
            }, async response => await response.ReadJsonAsync());
        }

        public async Task<JsonData> MongoRemove(string database, string collection, string id, string[] data)
        {
            return await MongoRequest("remove", new JsonData
            {
                { "database", database },
                { "collection", collection },
                { "id", id },
                { "data", data }
            }, async response => await response.ReadJsonAsync());
        }

        public async Task<JsonData> MongoDelete(string database, string collection, string id)
        {
            return await MongoRequest("delete", new JsonData
            {
                { "database", database },
                { "collection", collection },
                { "id", id }
            }, async response => await response.ReadJsonAsync());
        }
    }
}
