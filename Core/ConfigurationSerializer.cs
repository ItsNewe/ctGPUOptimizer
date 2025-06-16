using Newtonsoft.Json;
using OptimizationEngine.Models;

namespace OptimizationEngine.Core
{
    /// <summary>
    /// Handles serialization and deserialization of backtest configurations.
    /// </summary>
    public static class ConfigurationSerializer
    {
        /// <summary>
        /// Saves the given configurations to a JSON file.
        /// </summary>
        public static void SaveToFile(string filePath, IEnumerable<BacktestConfiguration> configs)
        {
            var json = JsonConvert.SerializeObject(configs, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads configurations from a JSON file.
        /// </summary>
        public static IEnumerable<BacktestConfiguration> LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Configuration file not found: {filePath}");
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<BacktestConfiguration>>(json)!;
        }
    }
}
