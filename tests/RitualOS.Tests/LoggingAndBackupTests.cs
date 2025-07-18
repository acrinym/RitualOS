using System.IO;
using System.Threading.Tasks;
using RitualOS.Services;

namespace RitualOS.Tests;

public class LoggingAndBackupTests
{
    [Fact]
    public async Task LoggingService_WritesFile()
    {
        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        if (Directory.Exists(logDir))
            Directory.Delete(logDir, true);
        await LoggingService.InfoAsync("test message");
        Assert.True(File.Exists(Path.Combine(logDir, "app.log")));
        var text = File.ReadAllText(Path.Combine(logDir, "app.log"));
        Assert.Contains("test message", text);
    }

    [Fact]
    public async Task BackupService_CreatesAndRestoresArchive()
    {
        var pwd = "password";
        var backupFile = Path.Combine(Path.GetTempPath(), "test_backup.rba");
        if (File.Exists(backupFile))
            File.Delete(backupFile);
        await BackupService.CreateBackupAsync(backupFile, pwd);
        Assert.True(File.Exists(backupFile));
        var restoreDir = Path.Combine(Path.GetTempPath(), "restore_test");
        if (Directory.Exists(restoreDir))
            Directory.Delete(restoreDir, true);
        Directory.CreateDirectory(restoreDir);
        await BackupService.RestoreBackupAsync(backupFile, pwd, restoreDir);
        Assert.True(Directory.GetFiles(restoreDir, "*", SearchOption.AllDirectories).Length > 0);
    }
}
