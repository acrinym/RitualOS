using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RitualOS.Services
{
    /// <summary>
    /// Provides encrypted backup and restore of RitualOS data directories.
    /// </summary>
    public static class BackupService
    {
        public static void CreateBackup(string outputPath, string password)
        {
            var tempZip = Path.GetTempFileName();
            if (File.Exists(tempZip))
                File.Delete(tempZip);

            ZipFile.CreateFromDirectory(AppContext.BaseDirectory, tempZip, CompressionLevel.Fastest, true);
            EncryptFile(tempZip, outputPath, password);
            File.Delete(tempZip);
        }

        public static void RestoreBackup(string archivePath, string password, string? restoreDirectory = null)
        {
            var tempZip = Path.GetTempFileName();
            DecryptFile(archivePath, tempZip, password);
            var target = restoreDirectory ?? AppContext.BaseDirectory;
            ZipFile.ExtractToDirectory(tempZip, target, true);
            File.Delete(tempZip);
        }

        /// <summary>
        /// Create a backup asynchronously. Great for heavy data! 😄
        /// </summary>
        public static Task CreateBackupAsync(string outputPath, string password)
        {
            return Task.Run(() => CreateBackup(outputPath, password));
        }

        /// <summary>
        /// Restore a backup asynchronously. 🧰
        /// </summary>
        public static Task RestoreBackupAsync(string archivePath, string password, string? restoreDirectory = null)
        {
            return Task.Run(() => RestoreBackup(archivePath, password, restoreDirectory));
        }

        private static void EncryptFile(string input, string output, string password)
        {
            using var aes = Aes.Create();
            var salt = RandomNumberGenerator.GetBytes(16);
            var key = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);
            using var fsOut = new FileStream(output, FileMode.Create);
            fsOut.Write(salt);
            using var crypto = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var fsIn = File.OpenRead(input);
            fsIn.CopyTo(crypto);
        }

        private static void DecryptFile(string input, string output, string password)
        {
            using var fsIn = File.OpenRead(input);
            var salt = new byte[16];
            fsIn.Read(salt, 0, 16);
            var key = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);
            using var crypto = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var fsOut = new FileStream(output, FileMode.Create);
            crypto.CopyTo(fsOut);
        }
    }
}