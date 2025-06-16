using ILGPU.Runtime;
using OptimizationEngine.Models;

namespace OptimizationEngine.GPU
{
    /// <summary>
    /// Coordinates GPU-accelerated backtesting using ILGPU.
    /// </summary>
    public class GPUBacktestManager
    {
        private readonly ILGPUManager _gpuManager;

        public GPUBacktestManager(ILGPUManager gpuManager)
        {
            _gpuManager = gpuManager;
        }

        /// <summary>
        /// Executes a batch of backtests in parallel using ILGPU.
        /// </summary>
        public IEnumerable<BacktestMetrics> RunBatch(double[] priceData, IEnumerable<BacktestConfiguration> configs)
        {
            var configList = configs.ToList();
            int backtestCount = configList.Count;
            int paramCount = configList.First().ParameterValues.Count;
            int priceLength = priceData.Length;
            // Flatten parameters
            var flatParams = configList.SelectMany(cfg => cfg.ParameterValues.Values.Select(v => Convert.ToDouble(v))).ToArray();

            // Allocate buffers
            using var priceBuffer = _gpuManager.Accelerator.Allocate1D<double>(priceData);
            using var paramBuffer = _gpuManager.Accelerator.Allocate1D<double>(flatParams);
            using var resultBuffer = _gpuManager.Accelerator.Allocate1D(new double[backtestCount * 4]);

            // Execute kernel
            _gpuManager.ExecuteBatch(backtestCount, priceBuffer, priceLength, paramBuffer, paramCount, resultBuffer);

            // Retrieve results
            var flatResults = resultBuffer.GetAsArray();
            for (int i = 0; i < backtestCount; i++)
            {
                yield return new BacktestMetrics
                {
                    Profit = flatResults[i * 4 + 0],
                    MaxDrawdown = flatResults[i * 4 + 1],
                    SharpeRatio = flatResults[i * 4 + 2],
                    WinRate = flatResults[i * 4 + 3]
                };
            }
        }
    }
}
