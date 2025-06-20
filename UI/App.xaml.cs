using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using OptimizationEngine.UI.Views;
using Avalonia.Markup.Xaml;

namespace OptimizationEngine.UI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
