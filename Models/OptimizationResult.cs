using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationEngine.Models
{
    /// <summary>
    /// Holds the result of a single backtest run.
    /// </summary>
    public class OptimizationResult
    {
        /// <summary>
        /// Configuration for this result (initialized non-null)
        /// </summary>
        public BacktestConfiguration Configuration { get; set; } = default!;

        /// <summary>
        /// Summary of parameter values as a semicolon-separated string.
        /// </summary>
        public string ParamSummary => string.Join("; ", Configuration.ParameterValues.Select(kv => $"{kv.Key}={kv.Value}"));

        // Performance metrics
        public double Profit { get; set; }
        public double MaxDrawdown { get; set; }
        public double SharpeRatio { get; set; }
        public double WinRate { get; set; }

        // Additional metrics
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();

        public DateTime RunTime { get; set; }
        
        //TODO: BacktestMetrics might be redundant, see later
    }
}
