using OptimizationEngine.Models;

namespace OptimizationEngine.Core
{
    /// <summary>
    /// Simulates cTrader lifecycle and trading environment: account balance, positions, orders, commissions, spreads.
    /// </summary>
    public class TradingSimulator
    {
        public double AccountBalance { get; private set; }
        public List<Trade> TradeHistory { get; } = new List<Trade>();

        private readonly BacktestConfiguration _config;

        public TradingSimulator(BacktestConfiguration config)
        {
            _config = config;
            AccountBalance = config.InitialBalance;
        }

        public void Initialize()
        {
            // reset state
            AccountBalance = _config.InitialBalance;
            TradeHistory.Clear();
        }

        public void OnTick(long timestamp, double price)
        {
            // Process tick: handle open positions, slippage, etc.
        }

        public void OnBar(long timestamp, double open, double high, double low, double close)
        {
            // Handle bar close events
        }

        public void PlaceOrder(Order order)
        {
            // Simulate order execution with spread and commission
        }

        // Additional methods for stop loss, take profit, margin checks, etc.
    }

    public class Trade
    {
        public DateTime Time { get; set; }
        public string Symbol { get; set; }
        public double Volume { get; set; }
        public double EntryPrice { get; set; }
        public double ExitPrice { get; set; }
        public double Profit { get; set; }
    }

    public class Order
    {
        public string Symbol { get; set; }
        public double Volume { get; set; }
        public bool IsBuy { get; set; }
        public double Price { get; set; }
        public double StopLoss { get; set; }
        public double TakeProfit { get; set; }
    }
}
