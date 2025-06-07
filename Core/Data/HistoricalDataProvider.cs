using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OptimizationEngine.Models;

namespace OptimizationEngine.Data
{
    /// <summary>
    /// Provides historical price data for backtesting, with local JSON caching.
    /// </summary>
    public class HistoricalDataProvider
    {
        private readonly CacheManager _cache;

        public HistoricalDataProvider(CacheManager cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Returns an array of closing prices for the given timeframe and date range.
        /// </summary>
        public double[] GetHistoricalData(string timeframe, DateTime start, DateTime end)
        {
            var cacheKey = $"{timeframe}_{start:yyyyMMdd}_{end:yyyyMMdd}";
            if (_cache.TryLoad(cacheKey, out double[] data))
                return data;

            // TODO: integrate with cTrader API to fetch real data.
            // For now, generate dummy sine-wave data.
            int days = (end - start).Days;
            var result = Enumerable.Range(0, days)
                .Select(i => Math.Sin(i * 2 * Math.PI / days) + 1)
                .ToArray();

            _cache.Save(cacheKey, result);
            return result;
        }
    }
}
