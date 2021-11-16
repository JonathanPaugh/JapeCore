using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace JapeHttp
{
    public class JsonData : IEnumerable
    {
        private readonly Dictionary<string, JsonElement> data;

        public JsonData()
        {
            data = new Dictionary<string, JsonElement>();
        }

        internal JsonData(string json)
        {
            data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        }

        public IEnumerator GetEnumerator() => data.GetEnumerator();

        public void Add(string key, object value) => data.Add(key, Json.DeserializeObject(value));
        public void Remove(string key) => data.Remove(key);

        public JsonElement Get(string key) => data[key];
        public byte GetByte(string key) => data[key].GetByte();
        public short GetShort(string key) => data[key].GetInt16();
        public int GetInt(string key) => data[key].GetInt32();
        public long GetLong(string key) => data[key].GetInt64();
        public float GetFloat(string key) => data[key].GetSingle();
        public double GetDouble(string key) => data[key].GetDouble();
        public string GetString(string key) => data[key].GetString();

        internal string Serialize() => JsonSerializer.Serialize(data);
    }
}
