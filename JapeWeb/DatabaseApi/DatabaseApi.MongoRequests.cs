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
    }
}
