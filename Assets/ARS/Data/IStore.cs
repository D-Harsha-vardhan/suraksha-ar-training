using System.Collections.Generic;
namespace ARS.Data { public interface IStore { void Save<T>(string key, T value); bool TryLoad<T>(string key, out T value); void Delete(string key); IReadOnlyCollection<string> Keys { get; } } }
