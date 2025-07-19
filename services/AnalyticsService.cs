using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RitualOS.Models;
using System.Text.Json;
using System.IO;

namespace RitualOS.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsData> GetAnalyticsAsync();
        Task TrackRitualUsageAsync(string ritualId);
        Task TrackFeatureUsageAsync(string featureName);
        Task TrackThemeUsageAsync(string themeName);
        Task TrackElementUsageAsync(string element);
        Task TrackChakraUsageAsync(string chakra);
        Task<Dictionary<string, int>> GetMostUsedRitualsAsync();
        Task<Dictionary<string, int>> GetElementUsageAsync();
        Task<Dictionary<string, int>> GetChakraUsageAsync();
        Task<Dictionary<string, int>> GetThemePreferencesAsync();
        Task<PerformanceMetrics> GetPerformanceMetricsAsync();
        Task ExportAnalyticsAsync(string format, string outputPath);
    }

    public class AnalyticsData
    {
        public DateTime Generated { get; set; } = DateTime.Now;
        public int TotalRituals { get; set; }
        public int TotalSessions { get; set; }
        public TimeSpan TotalUsageTime { get; set; }
        public Dictionary<string, int> MostUsedRituals { get; set; } = new();
        public Dictionary<string, int> ElementUsage { get; set; } = new();
        public Dictionary<string, int> ChakraUsage { get; set; } = new();
        public Dictionary<string, int> ThemePreferences { get; set; } = new();
        public Dictionary<string, int> FeatureUsage { get; set; } = new();
        public Dictionary<string, int> MoonPhaseFrequency { get; set; } = new();
        public Dictionary<string, int> IngredientUsage { get; set; } = new();
        public PerformanceMetrics Performance { get; set; } = new();
        public List<UsageSession> RecentSessions { get; set; } = new();
    }

    public class PerformanceMetrics
    {
        public double AverageStartupTime { get; set; }
        public double AverageMemoryUsage { get; set; }
        public int TotalCrashes { get; set; }
        public double CrashRate { get; set; }
        public Dictionary<string, double> FeatureResponseTimes { get; set; } = new();
    }

    public class UsageSession
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;
        public List<string> FeaturesUsed { get; set; } = new();
        public List<string> RitualsAccessed { get; set; } = new();
        public string ThemeUsed { get; set; } = string.Empty;
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly string _analyticsPath;
        private readonly string _sessionsPath;
        private readonly string _performancePath;
        private readonly IUserSettingsService _settingsService;

        public AnalyticsService(IUserSettingsService settingsService)
        {
            _settingsService = settingsService;
            _analyticsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "analytics.json");
            _sessionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "sessions.json");
            _performancePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "performance.json");
            
            InitializeAnalyticsFiles();
        }

        private void InitializeAnalyticsFiles()
        {
            var logsDir = Path.GetDirectoryName(_analyticsPath);
            if (!Directory.Exists(logsDir))
                Directory.CreateDirectory(logsDir!);

            if (!File.Exists(_analyticsPath))
            {
                var initialData = new AnalyticsData();
                File.WriteAllText(_analyticsPath, JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true }));
            }

            if (!File.Exists(_sessionsPath))
            {
                File.WriteAllText(_sessionsPath, JsonSerializer.Serialize(new List<UsageSession>(), new JsonSerializerOptions { WriteIndented = true }));
            }

            if (!File.Exists(_performancePath))
            {
                var initialPerformance = new PerformanceMetrics();
                File.WriteAllText(_performancePath, JsonSerializer.Serialize(initialPerformance, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public async Task<AnalyticsData> GetAnalyticsAsync()
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();

                // Update with current data
                analytics.MostUsedRituals = await GetMostUsedRitualsAsync();
                analytics.ElementUsage = await GetElementUsageAsync();
                analytics.ChakraUsage = await GetChakraUsageAsync();
                analytics.ThemePreferences = await GetThemePreferencesAsync();
                analytics.MoonPhaseFrequency = await GetMoonPhaseFrequencyAsync();
                analytics.IngredientUsage = await GetIngredientUsageAsync();
                analytics.Performance = await GetPerformanceMetricsAsync();
                analytics.RecentSessions = await GetRecentSessionsAsync();

                return analytics;
            }
            catch (Exception)
            {
                return new AnalyticsData();
            }
        }

        public async Task TrackRitualUsageAsync(string ritualId)
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();

                if (analytics.MostUsedRituals.ContainsKey(ritualId))
                    analytics.MostUsedRituals[ritualId]++;
                else
                    analytics.MostUsedRituals[ritualId] = 1;

                await File.WriteAllTextAsync(_analyticsPath, JsonSerializer.Serialize(analytics, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception)
            {
                // Silently fail for analytics tracking
            }
        }

        public async Task TrackFeatureUsageAsync(string featureName)
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();

                if (analytics.FeatureUsage.ContainsKey(featureName))
                    analytics.FeatureUsage[featureName]++;
                else
                    analytics.FeatureUsage[featureName] = 1;

                await File.WriteAllTextAsync(_analyticsPath, JsonSerializer.Serialize(analytics, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception)
            {
                // Silently fail for analytics tracking
            }
        }

        public async Task TrackThemeUsageAsync(string themeName)
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();

                if (analytics.ThemePreferences.ContainsKey(themeName))
                    analytics.ThemePreferences[themeName]++;
                else
                    analytics.ThemePreferences[themeName] = 1;

                await File.WriteAllTextAsync(_analyticsPath, JsonSerializer.Serialize(analytics, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception)
            {
                // Silently fail for analytics tracking
            }
        }

        public async Task TrackElementUsageAsync(string element)
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();

                if (analytics.ElementUsage.ContainsKey(element))
                    analytics.ElementUsage[element]++;
                else
                    analytics.ElementUsage[element] = 1;

                await File.WriteAllTextAsync(_analyticsPath, JsonSerializer.Serialize(analytics, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception)
            {
                // Silently fail for analytics tracking
            }
        }

        public async Task TrackChakraUsageAsync(string chakra)
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();

                if (analytics.ChakraUsage.ContainsKey(chakra))
                    analytics.ChakraUsage[chakra]++;
                else
                    analytics.ChakraUsage[chakra] = 1;

                await File.WriteAllTextAsync(_analyticsPath, JsonSerializer.Serialize(analytics, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception)
            {
                // Silently fail for analytics tracking
            }
        }

        public async Task<Dictionary<string, int>> GetMostUsedRitualsAsync()
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();
                return analytics.MostUsedRituals;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, int>> GetElementUsageAsync()
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();
                return analytics.ElementUsage;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, int>> GetChakraUsageAsync()
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();
                return analytics.ChakraUsage;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, int>> GetThemePreferencesAsync()
        {
            try
            {
                var analyticsJson = await File.ReadAllTextAsync(_analyticsPath);
                var analytics = JsonSerializer.Deserialize<AnalyticsData>(analyticsJson) ?? new AnalyticsData();
                return analytics.ThemePreferences;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        public Task<Dictionary<string, int>> GetMoonPhaseFrequencyAsync()
        {
            try
            {
                var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples");
                var loader = new RitualDataLoader();
                var rituals = loader.LoadAllRituals(dataDir);

                var results = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var ritual in rituals)
                {
                    var phase = string.IsNullOrWhiteSpace(ritual.MoonPhase) ? "Unknown" : ritual.MoonPhase;
                    if (results.ContainsKey(phase))
                        results[phase]++;
                    else
                        results[phase] = 1;
                }

                return Task.FromResult(results);
            }
            catch (Exception)
            {
                return Task.FromResult(new Dictionary<string, int>());
            }
        }

        public Task<Dictionary<string, int>> GetIngredientUsageAsync()
        {
            try
            {
                var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples");
                var loader = new RitualDataLoader();
                var rituals = loader.LoadAllRituals(dataDir);

                var results = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var ritual in rituals)
                {
                    foreach (var ing in ritual.Ingredients)
                    {
                        if (results.ContainsKey(ing))
                            results[ing]++;
                        else
                            results[ing] = 1;
                    }
                }

                return Task.FromResult(results);
            }
            catch (Exception)
            {
                return Task.FromResult(new Dictionary<string, int>());
            }
        }

        public async Task<PerformanceMetrics> GetPerformanceMetricsAsync()
        {
            try
            {
                var performanceJson = await File.ReadAllTextAsync(_performancePath);
                return JsonSerializer.Deserialize<PerformanceMetrics>(performanceJson) ?? new PerformanceMetrics();
            }
            catch (Exception)
            {
                return new PerformanceMetrics();
            }
        }

        private async Task<List<UsageSession>> GetRecentSessionsAsync()
        {
            try
            {
                var sessionsJson = await File.ReadAllTextAsync(_sessionsPath);
                var sessions = JsonSerializer.Deserialize<List<UsageSession>>(sessionsJson) ?? new List<UsageSession>();
                return sessions.OrderByDescending(s => s.StartTime).Take(10).ToList();
            }
            catch (Exception)
            {
                return new List<UsageSession>();
            }
        }

        public async Task ExportAnalyticsAsync(string format, string outputPath)
        {
            var analytics = await GetAnalyticsAsync();

            switch (format.ToLower())
            {
                case "json":
                    await File.WriteAllTextAsync(outputPath, JsonSerializer.Serialize(analytics, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case "markdown":
                    await ExportAnalyticsToMarkdownAsync(analytics, outputPath);
                    break;
                case "csv":
                    await ExportAnalyticsToCsvAsync(analytics, outputPath);
                    break;
                case "html":
                    await ExportAnalyticsToHtmlAsync(analytics, outputPath);
                    break;
                default:
                    throw new ArgumentException($"Unsupported format: {format}");
            }
        }

        private async Task ExportAnalyticsToMarkdownAsync(AnalyticsData analytics, string outputPath)
        {
            var markdown = new System.Text.StringBuilder();
            
            markdown.AppendLine("# RitualOS Analytics Report");
            markdown.AppendLine();
            markdown.AppendLine($"**Generated:** {analytics.Generated:yyyy-MM-dd HH:mm}  ");
            markdown.AppendLine($"**Total Rituals:** {analytics.TotalRituals}  ");
            markdown.AppendLine($"**Total Sessions:** {analytics.TotalSessions}  ");
            markdown.AppendLine($"**Total Usage Time:** {analytics.TotalUsageTime.TotalHours:F1} hours  ");
            markdown.AppendLine();
            
            // Most Used Rituals
            markdown.AppendLine("## Most Used Rituals");
            foreach (var ritual in analytics.MostUsedRituals.OrderByDescending(x => x.Value).Take(10))
            {
                markdown.AppendLine($"- **{ritual.Key}**: {ritual.Value} uses");
            }
            markdown.AppendLine();
            
            // Element Usage
            markdown.AppendLine("## Element Usage");
            foreach (var element in analytics.ElementUsage.OrderByDescending(x => x.Value))
            {
                markdown.AppendLine($"- **{element.Key}**: {element.Value} times");
            }
            markdown.AppendLine();
            
            // Chakra Usage
            markdown.AppendLine("## Chakra Usage");
            foreach (var chakra in analytics.ChakraUsage.OrderByDescending(x => x.Value))
            {
                markdown.AppendLine($"- **{chakra.Key}**: {chakra.Value} times");
            }
            markdown.AppendLine();

            // Moon Phase Frequency
            markdown.AppendLine("## Moon Phase Frequency");
            foreach (var phase in analytics.MoonPhaseFrequency.OrderByDescending(x => x.Value))
            {
                markdown.AppendLine($"- **{phase.Key}**: {phase.Value} rituals");
            }
            markdown.AppendLine();

            // Ingredient Usage
            markdown.AppendLine("## Ingredient Usage");
            foreach (var ing in analytics.IngredientUsage.OrderByDescending(x => x.Value))
            {
                markdown.AppendLine($"- **{ing.Key}**: {ing.Value} times");
            }
            markdown.AppendLine();
            
            // Theme Preferences
            markdown.AppendLine("## Theme Preferences");
            foreach (var theme in analytics.ThemePreferences.OrderByDescending(x => x.Value))
            {
                markdown.AppendLine($"- **{theme.Key}**: {theme.Value} sessions");
            }
            markdown.AppendLine();
            
            // Feature Usage
            markdown.AppendLine("## Feature Usage");
            foreach (var feature in analytics.FeatureUsage.OrderByDescending(x => x.Value))
            {
                markdown.AppendLine($"- **{feature.Key}**: {feature.Value} times");
            }
            markdown.AppendLine();
            
            // Performance Metrics
            markdown.AppendLine("## Performance Metrics");
            markdown.AppendLine($"- **Average Startup Time**: {analytics.Performance.AverageStartupTime:F2} seconds");
            markdown.AppendLine($"- **Average Memory Usage**: {analytics.Performance.AverageMemoryUsage:F2} MB");
            markdown.AppendLine($"- **Total Crashes**: {analytics.Performance.TotalCrashes}");
            markdown.AppendLine($"- **Crash Rate**: {analytics.Performance.CrashRate:P2}");
            markdown.AppendLine();
            
            await File.WriteAllTextAsync(outputPath, markdown.ToString());
        }

        private async Task ExportAnalyticsToCsvAsync(AnalyticsData analytics, string outputPath)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Metric,Name,Value");

            csv.AppendLine($"Summary,Total Rituals,{analytics.TotalRituals}");
            csv.AppendLine($"Summary,Total Sessions,{analytics.TotalSessions}");
            csv.AppendLine($"Summary,Total Usage Hours,{analytics.TotalUsageTime.TotalHours:F1}");

            foreach (var ritual in analytics.MostUsedRituals)
            {
                csv.AppendLine($"Most Used Rituals,{ritual.Key},{ritual.Value}");
            }
            foreach (var element in analytics.ElementUsage)
            {
                csv.AppendLine($"Element Usage,{element.Key},{element.Value}");
            }
            foreach (var chakra in analytics.ChakraUsage)
            {
                csv.AppendLine($"Chakra Usage,{chakra.Key},{chakra.Value}");
            }
            foreach (var phase in analytics.MoonPhaseFrequency)
            {
                csv.AppendLine($"Moon Phase Frequency,{phase.Key},{phase.Value}");
            }
            foreach (var ing in analytics.IngredientUsage)
            {
                csv.AppendLine($"Ingredient Usage,{ing.Key},{ing.Value}");
            }
            foreach (var theme in analytics.ThemePreferences)
            {
                csv.AppendLine($"Theme Preferences,{theme.Key},{theme.Value}");
            }
            foreach (var feature in analytics.FeatureUsage)
            {
                csv.AppendLine($"Feature Usage,{feature.Key},{feature.Value}");
            }

            await File.WriteAllTextAsync(outputPath, csv.ToString());
        }

        private async Task ExportAnalyticsToHtmlAsync(AnalyticsData analytics, string outputPath)
        {
            var html = new System.Text.StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>RitualOS Analytics Report</title>");
            html.AppendLine("    <style>");
            html.AppendLine(GetAnalyticsCss());
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            html.AppendLine("    <header>");
            html.AppendLine("        <h1>RitualOS Analytics Report</h1>");
            html.AppendLine($"        <p>Generated on {analytics.Generated:yyyy-MM-dd HH:mm}</p>");
            html.AppendLine("    </header>");
            
            html.AppendLine("    <main>");
            
            // Summary
            html.AppendLine("        <section class=\"summary\">");
            html.AppendLine("            <h2>Summary</h2>");
            html.AppendLine("            <div class=\"summary-grid\">");
            html.AppendLine($"                <div class=\"summary-item\">");
            html.AppendLine($"                    <h3>{analytics.TotalRituals}</h3>");
            html.AppendLine($"                    <p>Total Rituals</p>");
            html.AppendLine($"                </div>");
            html.AppendLine($"                <div class=\"summary-item\">");
            html.AppendLine($"                    <h3>{analytics.TotalSessions}</h3>");
            html.AppendLine($"                    <p>Total Sessions</p>");
            html.AppendLine($"                </div>");
            html.AppendLine($"                <div class=\"summary-item\">");
            html.AppendLine($"                    <h3>{analytics.TotalUsageTime.TotalHours:F1}</h3>");
            html.AppendLine($"                    <p>Hours Used</p>");
            html.AppendLine($"                </div>");
            html.AppendLine("            </div>");
            html.AppendLine("        </section>");
            
            // Most Used Rituals
            html.AppendLine("        <section>");
            html.AppendLine("            <h2>Most Used Rituals</h2>");
            html.AppendLine("            <div class=\"chart\">");
            foreach (var ritual in analytics.MostUsedRituals.OrderByDescending(x => x.Value).Take(10))
            {
                var percentage = analytics.MostUsedRituals.Values.Max() > 0 ? (double)ritual.Value / analytics.MostUsedRituals.Values.Max() * 100 : 0;
                html.AppendLine($"                <div class=\"chart-item\">");
                html.AppendLine($"                    <span class=\"label\">{ritual.Key}</span>");
                html.AppendLine($"                    <div class=\"bar\">");
                html.AppendLine($"                        <div class=\"bar-fill\" style=\"width: {percentage}%\"></div>");
                html.AppendLine($"                    </div>");
                html.AppendLine($"                    <span class=\"value\">{ritual.Value}</span>");
                html.AppendLine($"                </div>");
            }
            html.AppendLine("            </div>");
            html.AppendLine("        </section>");

            // Moon Phase Frequency
            html.AppendLine("        <section>");
            html.AppendLine("            <h2>Moon Phase Frequency</h2>");
            html.AppendLine("            <ul>");
            foreach (var phase in analytics.MoonPhaseFrequency.OrderByDescending(x => x.Value))
            {
                html.AppendLine($"                <li><strong>{phase.Key}</strong>: {phase.Value} rituals</li>");
            }
            html.AppendLine("            </ul>");
            html.AppendLine("        </section>");

            // Ingredient Usage
            html.AppendLine("        <section>");
            html.AppendLine("            <h2>Ingredient Usage</h2>");
            html.AppendLine("            <ul>");
            foreach (var ing in analytics.IngredientUsage.OrderByDescending(x => x.Value))
            {
                html.AppendLine($"                <li><strong>{ing.Key}</strong>: {ing.Value} times</li>");
            }
            html.AppendLine("            </ul>");
            html.AppendLine("        </section>");
            
            html.AppendLine("    </main>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            await File.WriteAllTextAsync(outputPath, html.ToString());
        }

        private string GetAnalyticsCss()
        {
            return @"
                body {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    line-height: 1.6;
                    margin: 0;
                    padding: 20px;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    min-height: 100vh;
                }
                
                header {
                    background: rgba(255, 255, 255, 0.95);
                    padding: 30px;
                    border-radius: 15px;
                    margin-bottom: 30px;
                    text-align: center;
                }
                
                h1 {
                    color: #2d3748;
                    margin: 0 0 10px 0;
                }
                
                section {
                    background: rgba(255, 255, 255, 0.95);
                    padding: 25px;
                    border-radius: 15px;
                    margin-bottom: 25px;
                }
                
                h2 {
                    color: #2d3748;
                    border-bottom: 3px solid #667eea;
                    padding-bottom: 10px;
                    margin-bottom: 20px;
                }
                
                .summary-grid {
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
                    gap: 20px;
                }
                
                .summary-item {
                    text-align: center;
                    padding: 20px;
                    background: #f7fafc;
                    border-radius: 10px;
                }
                
                .summary-item h3 {
                    font-size: 2em;
                    color: #667eea;
                    margin: 0 0 10px 0;
                }
                
                .chart-item {
                    display: flex;
                    align-items: center;
                    margin: 10px 0;
                    gap: 15px;
                }
                
                .label {
                    min-width: 150px;
                    font-weight: bold;
                }
                
                .bar {
                    flex: 1;
                    height: 20px;
                    background: #e2e8f0;
                    border-radius: 10px;
                    overflow: hidden;
                }
                
                .bar-fill {
                    height: 100%;
                    background: linear-gradient(90deg, #667eea, #764ba2);
                    transition: width 0.3s ease;
                }
                
                .value {
                    min-width: 50px;
                    text-align: right;
                    font-weight: bold;
                    color: #2d3748;
                }
            ";
        }
    }
} 