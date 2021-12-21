using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace JapeCore
{
    public class JsonData : IEnumerable
    {
        private readonly Dictionary<string, JsonElement> data;

        public JsonData()
        {
            data = new Dictionary<string, JsonElement>();
        }

        public JsonData(JsonData data)
        {
            this.data = Deserialize(data.Serialize());
        }

        internal JsonData(string json)
        {
            data = Deserialize(json);
        }

        internal string Serialize() => JsonSerializer.Serialize(data);

        public T Construct<T>() where T : IJsonConstructable, new()
        {
            return JsonSerializer.Deserialize<T>(Serialize(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });
        }

        public IEnumerator GetEnumerator() => data.GetEnumerator();

        public void Add(string key, object value) => data.Add(key, DeserializeElement(value));

        public void Remove(string key) => data.Remove(key);
        
        public byte GetByte(string key) => data[key].GetByte();
        public sbyte GetSByte(string key) => data[key].GetSByte();
        public bool GetBool(string key) => data[key].GetBoolean();
        public short GetShort(string key) => data[key].GetInt16();
        public ushort GetUShort(string key) => data[key].GetUInt16();
        public int GetInt(string key) => data[key].GetInt32();
        public uint GetUInt(string key) => data[key].GetUInt32();
        public long GetLong(string key) => data[key].GetInt64();
        public ulong GetULong(string key) => data[key].GetUInt64();
        public float GetFloat(string key) => data[key].GetSingle();
        public double GetDouble(string key) => data[key].GetDouble();
        public decimal GetDecimal(string key) => data[key].GetDecimal();
        public string GetString(string key) => data[key].GetString();
    
        public IEnumerable<byte> GetByteArray(string key) => data[key].EnumerateArray().Select(e => e.GetByte());
        public IEnumerable<sbyte> GetSByteArray(string key) => data[key].EnumerateArray().Select(e => e.GetSByte());
        public IEnumerable<bool> GetBoolArray(string key) => data[key].EnumerateArray().Select(e => e.GetBoolean());
        public IEnumerable<short> GetShortArray(string key) => data[key].EnumerateArray().Select(e => e.GetInt16());
        public IEnumerable<ushort> GetUShortArray(string key) => data[key].EnumerateArray().Select(e => e.GetUInt16());
        public IEnumerable<int> GetIntArray(string key) => data[key].EnumerateArray().Select(e => e.GetInt32());
        public IEnumerable<uint> GetUIntArray(string key) => data[key].EnumerateArray().Select(e => e.GetUInt32());
        public IEnumerable<long> GetLongArray(string key) => data[key].EnumerateArray().Select(e => e.GetInt64());
        public IEnumerable<ulong> GetULongArray(string key) => data[key].EnumerateArray().Select(e => e.GetUInt64());
        public IEnumerable<float> GetFloatArray(string key) => data[key].EnumerateArray().Select(e => e.GetSingle());
        public IEnumerable<double> GetDoubleArray(string key) => data[key].EnumerateArray().Select(e => e.GetDouble());
        public IEnumerable<decimal> GetDecimalArray(string key) => data[key].EnumerateArray().Select(e => e.GetDecimal());
        public IEnumerable<string> GetStringArray(string key) => data[key].EnumerateArray().Select(e => e.GetString());

        public JsonData Extract(string key, params string[] properties) => new(Find(key, properties).GetRawText());
        public IEnumerable<JsonData> ExtractArray(string key, params string[] properties) => Find(key, properties).EnumerateArray().Select(e => new JsonData(e.GetRawText()));

        private JsonElement Find(string key, params string[] properties)
        {
            JsonElement element = data[key];

            foreach (string property in properties)
            {
                element = element.GetProperty(property);
            }

            return element;
        }

        private static Dictionary<string, JsonElement> Deserialize(string data)
        {
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(data);
        }

        private static JsonElement DeserializeElement(object data)
        {
            if (data.GetType() == typeof(JsonData))
            {
                data = ((JsonData)data).data;
            }

            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(data);
            Utf8JsonReader reader = new(bytes);
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.Clone();
        }

        public BsonDocument ToBson() => BsonDocument.Parse(Serialize());
    }
}
