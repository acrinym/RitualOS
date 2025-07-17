using System;
using System.Security.Cryptography;
using System.Text;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// ViewModel for the initial welcome screen handling token access.
    /// </summary>
    public class WelcomeViewModel : ViewModelBase
    {
        private string _token = string.Empty;
        private string _status = string.Empty;
        private const string PublicKeyPem = "-----BEGIN PUBLIC KEY-----\nMFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBANIpn0ycxB0wOq/QgxWnLd6XCGAOYl9I\nCvidFbcypcPkl8XXsX9HP28Bgcl5lTevVZ3Ca43IlG0CCzplqGEBq9MCAwEAAQ==\n-----END PUBLIC KEY-----";

        public string Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        public string StatusMessage
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public RelayCommand SubmitCommand { get; }
        public RelayCommand SuperuserCommand { get; }

        public Action? AccessGranted { get; set; }

        public WelcomeViewModel()
        {
            SubmitCommand = new RelayCommand(_ => VerifyToken());
            SuperuserCommand = new RelayCommand(_ =>
            {
                UserContext.CurrentRole = Role.Admin;
                AccessGranted?.Invoke();
            });
        }

        private void VerifyToken()
        {
            foreach (var roleName in Enum.GetNames(typeof(Role)))
            {
                if (VerifySignature($"ROLE:{roleName}", Token))
                {
                    UserContext.CurrentRole = Enum.Parse<Role>(roleName);
                    StatusMessage = $"Welcome, {roleName}!";
                    AccessGranted?.Invoke();
                    return;
                }
            }
            StatusMessage = "Invalid token";
        }

        private bool VerifySignature(string message, string token)
        {
            try
            {
                using RSA rsa = RSA.Create();
                rsa.ImportFromPem(PublicKeyPem.ToCharArray());
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] sig = Convert.FromBase64String(token);
                return rsa.VerifyData(data, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch
            {
                return false;
            }
        }
    }
}
