using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RitualOS.ViewModels;
using RitualOS.Views;

namespace RitualOS
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
                desktop.MainWindow = new Window
                {
                    Content = new InventoryView() // Fixed: Use InventoryView (UserControl), not InventoryViewModel
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }

    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting RitualOS..."); // Added for debugging
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        private static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
        }
    }
}