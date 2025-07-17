using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RitualOS.ViewModels;
using RitualOS.Views;
using RitualOS.Services;

namespace RitualOS
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            LoggingService.Info("Starting RitualOS...");
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