using System;
using System.Collections.Generic;

namespace OptimizationEngine.Models
{
    /// <summary>
    /// Represents a single backtest configuration for a cBot with specified parameter values.
    /// </summary>
    public class BacktestConfiguration
    {
        /// <summary>
        /// Path to the compiled cBot assembly (.dll).
        /// </summary>
        public string BotAssemblyPath { get; set; }

        /// <summary>
        /// Fully-qualified type name of the cBot class.
        /// </summary>
        public string BotTypeName { get; set; }

        /// <summary>
        /// Dictionary mapping parameter property names to chosen values.
        /// </summary>
        public Dictionary<string, object> ParameterValues { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Timeframe for backtesting ("1m", "15m", "1h", etc.).
        /// </summary>
        public string TimeFrame { get; set; }
        
        /// <summary>
        /// Initial account balance for the backtest.
        /// </summary>
        public double InitialBalance { get; set; } = 40000.0;

        /// <summary>
        /// Backtest start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Backtest end date.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
