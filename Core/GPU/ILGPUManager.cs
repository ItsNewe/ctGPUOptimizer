using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.OpenCL;

namespace OptimizationEngine.GPU
{
    /// <summary>
    /// Manages ILGPU context, accelerator, and kernel execution.
    /// </summary>
    public class ILGPUManager : IDisposable
    {
        private readonly Context _context;
        private readonly Accelerator _accelerator;
        private readonly Action<Index1D, ArrayView<double>, int, ArrayView<double>, int, ArrayView<double>> _kernel;

        public Accelerator Accelerator => _accelerator;

        public ILGPUManager()
        {
            _context = Context.CreateDefault();
			// Create default accelerator (GPU if available, otherwise CPU)
			_accelerator = _context.CreateCLAccelerator(0);
            // Load kernel
            _kernel = _accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<double>, int, ArrayView<double>, int, ArrayView<double>>(RunBacktests);
        }

        /// <summary>
        /// Executes the backtest kernel for each configuration.
        /// </summary>
        public void ExecuteBatch(Index1D launchSize, MemoryBuffer<double> priceBuffer, int priceLength,
                                 MemoryBuffer<double> paramBuffer, int paramCount,
                                 MemoryBuffer<double> resultBuffer)
        {
            _kernel(launchSize, priceBuffer.View, priceLength, paramBuffer.View, paramCount, resultBuffer.View);
            _accelerator.Synchronize();
        }

        public static void RunBacktests(Index1D index, ArrayView<double> priceData, int priceLength,
                                        ArrayView<double> paramsView, int paramCount,
                                        ArrayView<double> results)
        {
            int gid = index;
            if (gid >= results.Length / 4)
                return;

            // Placeholder: compute profit as price change
            double profit = priceData[priceLength - 1] - priceData[0];
            double maxDrawdown = 0.0;
            double sharpeRatio = 0.0;
            double winRate = 0.0;

            int offset = gid * 4;
            results[offset] = profit;
            results[offset + 1] = maxDrawdown;
            results[offset + 2] = sharpeRatio;
            results[offset + 3] = winRate;
        }

        public void Dispose()
        {
            _accelerator.Dispose();
            _context.Dispose();
        }
    }
}
