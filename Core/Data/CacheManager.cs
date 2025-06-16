using System;
using System.IO;
using Newtonsoft.Json;

namespace OptimizationEngine.Data
{
    /// <summary>
    /// Simple file-based JSON caching.
    /// </summary>
    public class CacheManager
    {
        private readonly string _cacheFolder;

        public CacheManager(string cacheFolder = null)
        {
            _cacheFolder = cacheFolder ?? Path.Combine(Environment.CurrentDirectory, "cache");
            if (!Directory.Exists(_cacheFolder))
                Directory.CreateDirectory(_cacheFolder);
        }

        public bool TryLoad<T>(string key, out T data)
        {
            var file = Path.Combine(_cacheFolder, key + ".json");
            if (!File.Exists(file))
            {
                data = default;
                return false;
            }
            var json = File.ReadAllText(file);
            data = JsonConvert.DeserializeObject<T>(json);
            return true;
        }

        public void Save<T>(string key, T data)
        {
            var file = Path.Combine(_cacheFolder, key + ".json");
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(file, json);
        }
    }
}
