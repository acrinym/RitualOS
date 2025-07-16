using System;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace RitualOS
{
    internal class Program
    {
        // Main entry point of the application
        [STAThread]
        public static void Main(string[] args)
        {
            // Check for the --headless argument to run in a non-UI environment like Codespaces
            if (args.Contains("--headless"))
            {
                Console.WriteLine("Running in headless mode...");
                BuildAvaloniaApp()
                    .StartHeadless(args);
            }
            else
            {
                // This is the normal startup for a desktop environment (like Windows)
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
    
    // Custom extension class for running in a headless environment
    public static class HeadlessPlatformExtensions
    {
        public static int StartHeadless(this AppBuilder builder, string[] args)
        {
            var options = new AvaloniaHeadlessPlatformOptions();
            
            // This tells Avalonia to use the headless platform
            builder.UseHeadless(options);

            // The headless platform needs a windowing platform to function
            builder.UseSkia();
            
            builder.AfterSetup(_ =>
            {
                // This block runs after the app is initialized
                Dispatcher.UIThread.Post(() =>
                {
                    // Find the main window
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                        desktop.MainWindow is Window window)
                    {
                        // Give it a moment to render, then take a screenshot
                        Dispatcher.UIThread.Post(async () =>
                        {
                            await System.Threading.Tasks.Task.Delay(100); // Wait 100ms for rendering
                            TakeScreenshot(window);
                            desktop.Shutdown();
                        }, DispatcherPriority.Background);
                    }
                }, DispatcherPriority.Background);
            });

            builder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
            return 0;
        }

        private static void TakeScreenshot(Window window)
        {
            var pixelSize = new PixelSize((int)window.Width, (int)window.Height);
            var size = new Size(window.Width, window.Height);
            using var bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
            
            window.Measure(size);
            window.Arrange(new Rect(size));
            
            bitmap.Render(window);

            var outputPath = "output.png";
            bitmap.Save(outputPath);
            Console.WriteLine($"Screenshot saved to {Path.GetFullPath(outputPath)}");
        }
    }
}