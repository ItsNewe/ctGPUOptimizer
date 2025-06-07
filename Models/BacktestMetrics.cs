namespace OptimizationEngine.Models
{
    /// <summary>
    /// Performance metrics result of a backtest.
    /// </summary>
    public struct BacktestMetrics
    {
        public double Profit;
        public double MaxDrawdown;
        public double SharpeRatio;
        public double WinRate;
        //TODO: Detailed metrics, precise history of position actions (sl modification, etc..)
    }
}