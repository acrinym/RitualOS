using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RitualOS.ViewModels;
using RitualOS.Views;

namespace RitualOS
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // check SigilLock before allowing main window to appear
                if (!Services.SigilLock.HasAccess(Services.UserContext.CurrentRole, "AppAccess"))
                {
                    Console.WriteLine("Access denied: insufficient role for main UI.");
                    desktop.Shutdown();
                    return;
                }

                desktop.MainWindow = new Window
                {
                    Title = "RitualOS",
                    Width = 1200,
                    Height = 800,
                    Content = new MainShellView()
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
} 