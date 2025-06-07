using System;
using System.Collections.Generic;

namespace OptimizationEngine.Models
{
    /// <summary>
    /// Holds the result of a single backtest run.
    /// </summary>
    public class OptimizationResult
    {
        public BacktestConfiguration Configuration { get; set; }

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
