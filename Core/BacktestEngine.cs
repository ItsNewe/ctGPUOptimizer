using OptimizationEngine.Models;

namespace OptimizationEngine.Core
{
    /// <summary>
    /// Executes a complete backtest of a cBot instance with given configuration.
    /// </summary>
    public class BacktestEngine
    {
        private readonly TradingSimulator _simulator;

        public BacktestEngine(TradingSimulator simulator)
        {
            _simulator = simulator;
        }

        /// <summary>
        /// Runs a backtest for the specified cBot type and configuration.
        /// </summary>
        public OptimizationResult Run(Type cBotType, BacktestConfiguration config, double[] priceData)
        {
            // create cBot instance
            var bot = Activator.CreateInstance(cBotType);
            // set parameter values
            foreach (var kv in config.ParameterValues)
            {
                var prop = cBotType.GetProperty(kv.Key);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(bot, Convert.ChangeType(kv.Value, prop.PropertyType));
            }
            // initialize simulator
            var simulator = new TradingSimulator(config);
            simulator.Initialize();
            // call OnStart
            cBotType.GetMethod("OnStart")?.Invoke(bot, null);
            // run through price data as ticks
            for (int i = 0; i < priceData.Length; i++)
            {
                long timestamp = config.StartDate.Ticks + i;
                simulator.OnTick(timestamp, priceData[i]);
                cBotType.GetMethod("OnTick")?.Invoke(bot, null);
            }
            // call OnStop
            cBotType.GetMethod("OnStop")?.Invoke(bot, null);
            // calculate metrics
            var trades = simulator.TradeHistory;
            double profit = 0, maxDrawdown = 0, winCount = 0;
            foreach (var t in trades)
            {
                profit += t.Profit;
                if (t.Profit > 0) winCount++;
                // TODO: compute drawdown sequence
            }
            double winRate = trades.Count > 0 ? winCount / trades.Count : 0;
            // TODO: compute sharpe ratio
            var result = new OptimizationResult
            {
                Configuration = config,
                Profit = profit,
                MaxDrawdown = maxDrawdown,
                SharpeRatio = 0,
                WinRate = winRate,
                RunTime = DateTime.Now
            };
            return result;
        }
    }
}
