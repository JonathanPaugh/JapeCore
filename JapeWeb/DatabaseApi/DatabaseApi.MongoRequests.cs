using JapeHttp;

namespace JapeWeb
{
    public partial class DatabaseApi
    {
        private ApiResponse MongoRequest(string command, JsonData data)
        {
            JsonData mongoData = new(data)
            {
                { "index", MongoIndex },
                { "id", command }
            };
            return Request(mongoData);
        }

        public string MongoGet(string database, string collection, string id)
        {
            return MongoRequest("get", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "key", id }
            }).Read();
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
            }).Read();
        }

        public string MongoUpdate(string database, string collection, string id, JsonData data)
        {
            return MongoRequest("update", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "key", id },
                { "data", data }
            }).Read();
        }

        public string MongoRemove(string database, string collection, string id, string[] data)
        {
            return MongoRequest("remove", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "key", id },
                { "data", data }
            }).Read();
        }

        public string MongoDelete(string database, string collection, string id)
        {
            return MongoRequest("delete", new JsonData
            {
                { "store", database },
                { "collection", collection },
                { "key", id }
            }).Read();
        }
    }
}
