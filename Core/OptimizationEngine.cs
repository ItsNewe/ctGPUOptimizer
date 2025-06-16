using OptimizationEngine.Models;
using OptimizationEngine.GPU;
using OptimizationEngine.Data;

namespace OptimizationEngine.Core
{
    /// <summary>
    /// Main optimization engine: parameter space generation and backtesting orchestration.
    /// </summary>
    public class OptimizationEngine
    {
        private readonly ILGPUManager _gpu;
        private readonly HistoricalDataProvider _dataProvider;

        public OptimizationEngine(ILGPUManager gpu, HistoricalDataProvider dataProvider)
        {
            _gpu = gpu;
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Runs a grid search over all parameter combinations for the specified cBot.
        /// </summary>
        public IEnumerable<OptimizationResult> RunGridSearch(string assemblyPath, string typeName,
            DateTime start, DateTime end, string timeframe)
        {
            var asm = cBotLoader.LoadAssembly(assemblyPath);
            var botType = asm.GetType(typeName) ?? throw new InvalidOperationException($"Type {typeName} not found.");
            var paramDefs = ParameterExtractor.Extract(botType).ToList();

            // generate parameter combinations
            var valueDicts = ParameterSpaceGenerator.Generate(paramDefs);
            // create configurations
            var configs = valueDicts.Select(dict => new BacktestConfiguration
            {
                BotAssemblyPath = assemblyPath,
                BotTypeName = typeName,
                ParameterValues = dict,
                TimeFrame = timeframe,
                StartDate = start,
                EndDate = end
            }).ToList();

            // fetch historical data
            var priceData = _dataProvider.GetHistoricalData(timeframe, start, end);

            var resultsList = new List<OptimizationResult>();
            var gpuRunner = new GPUBacktestManager(_gpu);
            var metrics = gpuRunner.RunBatch(priceData, configs).ToList();
            for (int i = 0; i < configs.Count; i++)
            {
                var m = metrics[i];
                resultsList.Add(new OptimizationResult
                {
                    Configuration = configs[i],
                    Profit = m.Profit,
                    MaxDrawdown = m.MaxDrawdown,
                    SharpeRatio = m.SharpeRatio,
                    WinRate = m.WinRate,
                    RunTime = DateTime.Now
                });
            }

            return resultsList.OrderByDescending(r => r.Profit)
                               .ThenByDescending(r => r.SharpeRatio)
                               .ToList();
        }

        private OptimizationResult RunSingle(BacktestConfiguration config, double[] priceData)
        {
            var engine = new BacktestEngine(new TradingSimulator(config));
            var result = engine.Run(Type.GetType(config.BotTypeName)!, config, priceData);
            return result;
        }
    }
}
