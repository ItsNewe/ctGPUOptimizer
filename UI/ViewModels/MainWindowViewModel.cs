using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using OptimizationEngine.Core;
using OptimizationEngine.Data;
using OptimizationEngine.GPU;
using OptimizationEngine.Models;

namespace OptimizationEngine.UI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        // Services
        private readonly OptimizationEngine.Core.OptimizationEngine _engine;

        // Input fields
        private string _botAssemblyPath = string.Empty;
        private string _botTypeName = string.Empty;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _selectedTimeframe = string.Empty;

        // Collections
        public ObservableCollection<ParameterViewModel> Parameters { get; } = new ObservableCollection<ParameterViewModel>();
        public ObservableCollection<OptimizationResult> Results { get; } = new ObservableCollection<OptimizationResult>();
        public ObservableCollection<string> BotTypes { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Timeframes { get; } = new ObservableCollection<string> { "Minute", "Hour", "Daily", "Weekly" };

        // UI bindings
        public string SelectedBotType
        {
            get => BotTypeName;
            set => this.RaiseAndSetIfChanged(ref _botTypeName, value);
        }
        public string BotAssemblyPath
        {
            get => _botAssemblyPath;
            set => this.RaiseAndSetIfChanged(ref _botAssemblyPath, value);
        }
        public string BotTypeName
        {
            get => _botTypeName;
            set => this.RaiseAndSetIfChanged(ref _botTypeName, value);
        }
        public DateTime StartDate
        {
            get => _startDate;
            set => this.RaiseAndSetIfChanged(ref _startDate, value);
        }
        public DateTime EndDate
        {
            get => _endDate;
            set => this.RaiseAndSetIfChanged(ref _endDate, value);
        }
        public string SelectedTimeframe
        {
            get => _selectedTimeframe;
            set => this.RaiseAndSetIfChanged(ref _selectedTimeframe, value);
        }
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set => this.RaiseAndSetIfChanged(ref _isRunning, value);
        }

        // Commands
        public ReactiveCommand<Unit, Unit> LoadParametersCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveConfigCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadConfigFileCommand { get; }
        public ReactiveCommand<Unit, Unit> RunOptimizationCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportResultsCommand { get; }

        public MainWindowViewModel()
        {
            // initialize dates
            StartDate = DateTime.Today.AddMonths(-1);
            EndDate = DateTime.Today;
            SelectedTimeframe = Timeframes.First();

            // setup engine
            var cache = new CacheManager();
            var dataProvider = new HistoricalDataProvider(cache);
            var gpuManager = new ILGPUManager();
            _engine = new OptimizationEngine.Core.OptimizationEngine(gpuManager, dataProvider);

            LoadParametersCommand = ReactiveCommand.Create(LoadParameters);
            SaveConfigCommand = ReactiveCommand.Create(SaveConfig, this.WhenAnyValue(x => x.Parameters.Count).Select(c => c > 0));
            LoadConfigFileCommand = ReactiveCommand.Create(LoadConfigFile);
            RunOptimizationCommand = ReactiveCommand.CreateFromTask(RunOptimization, this.WhenAnyValue(x => x.Parameters.Count).Select(c => c > 0));
            ExportResultsCommand = ReactiveCommand.Create(ExportResults, this.WhenAnyValue(x => x.Results.Count).Select(c => c > 0));
        }

        private void LoadParameters()
        {
            if (!File.Exists(BotAssemblyPath)) return;
            try
            {
                var asm = cBotLoader.LoadAssembly(BotAssemblyPath);
                // populate available bot types
                BotTypes.Clear();
                foreach (var t in asm.GetTypes().Where(t => t.IsClass))
                    BotTypes.Add(t.FullName!);
                SelectedBotType = BotTypes.FirstOrDefault(x => x.EndsWith("cBot"))
                    ?? BotTypes.FirstOrDefault() ?? string.Empty;
                var botType = !string.IsNullOrWhiteSpace(SelectedBotType)
                    ? asm.GetType(SelectedBotType)
                    : null;
                if (botType == null) return;

                var defs = ParameterExtractor.Extract(botType).ToList();
                Parameters.Clear();
                foreach (var def in defs)
                    Parameters.Add(new ParameterViewModel(def));
            }
            catch { /* log or ignore */ }
        }

        private async Task RunOptimization()
        {
            IsRunning = true;
            Results.Clear();

            // Prepare config
            var configs = Parameters.Select(p => new { p.PropertyName, p.Value }).ToList();
            // Only single config: take current values
            var backtestConfig = new BacktestConfiguration
            {
                BotAssemblyPath = BotAssemblyPath,
                BotTypeName = BotTypeName,
                StartDate = StartDate,
                EndDate = EndDate,
                TimeFrame = SelectedTimeframe,
                ParameterValues = configs.ToDictionary(x => x.PropertyName, x => x.Value)
            };

            await Task.Run(() =>
            {
                var res = _engine.RunGridSearch(backtestConfig.BotAssemblyPath, backtestConfig.BotTypeName,
                    backtestConfig.StartDate, backtestConfig.EndDate, backtestConfig.TimeFrame);
                foreach (var r in res)
                {
                    RxApp.MainThreadScheduler.Schedule(Unit.Default, (_, __) =>
                    {
                        Results.Add(r);
                        return Disposable.Empty;
                    });
                }
            });

            IsRunning = false;
        }

        private void ExportResults()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "optimization_results.csv");
            var lines = new List<string> { "Profit,MaxDrawdown,SharpeRatio,WinRate" };
            lines.AddRange(Results.Select(r => $"{r.Profit},{r.MaxDrawdown},{r.SharpeRatio},{r.WinRate}"));
            File.WriteAllLines(path, lines);
        }

        private void SaveConfig()
        {
            var dict = Parameters.ToDictionary(p => p.PropertyName, p => p.Value);
            var config = new BacktestConfiguration
            {
                BotAssemblyPath = BotAssemblyPath,
                BotTypeName = BotTypeName,
                TimeFrame = SelectedTimeframe,
                StartDate = StartDate,
                EndDate = EndDate,
                ParameterValues = dict
            };
            var path = Path.Combine(Environment.CurrentDirectory, "parameters_config.json");
            ConfigurationSerializer.SaveToFile(path, new[] { config });
        }

        private void LoadConfigFile()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "parameters_config.json");
            if (!File.Exists(path)) return;
            var configs = ConfigurationSerializer.LoadFromFile(path).ToList();
            var cfg = configs.FirstOrDefault();
            if (cfg == null) return;
            BotAssemblyPath = cfg.BotAssemblyPath;
            BotTypeName = cfg.BotTypeName;
            SelectedTimeframe = cfg.TimeFrame;
            StartDate = cfg.StartDate;
            EndDate = cfg.EndDate;
            // update parameters list
            foreach (var p in Parameters)
            {
                if (cfg.ParameterValues.TryGetValue(p.PropertyName, out var val))
                    p.Value = val;
            }
        }
    }
}
