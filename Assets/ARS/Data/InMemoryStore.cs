using System.Collections.Generic;
namespace ARS.Data
{
    public sealed class InMemoryStore : IStore
    {
        private readonly Dictionary<string, object> data = new();
        public IReadOnlyCollection<string> Keys => data.Keys;
        public void Save<T>(string key, T value) => data[key] = value;
        public bool TryLoad<T>(string key, out T value)
        {
            if (data.TryGetValue(key, out var raw) && raw is T typed) { value = typed; return true; }
            value = default; return false;
        }
        public void Delete(string key) => data.Remove(key);
    }
}
