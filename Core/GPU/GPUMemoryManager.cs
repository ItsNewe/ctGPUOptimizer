namespace OptimizationEngine.GPU
{
    [System.Obsolete("GPUMemoryManager (OpenCL) is deprecated. Use ILGPUManager for GPU buffers.")]
    public class GPUMemoryManager : System.IDisposable
    {
        public GPUMemoryManager(object gpuManager) { }
        public void Initialize(double[] priceData, System.Collections.Generic.IEnumerable<object> configs) { }
        public void Release() { }
        public void Dispose() => Release();
        public int ParamCount => 0;
        public int BacktestCount => 0;
        public int PriceLength => 0;
        public IntPtr PriceBuffer => IntPtr.Zero;
        public IntPtr ParamBuffer => IntPtr.Zero;
        public IntPtr ResultBuffer => IntPtr.Zero;
    }
}
