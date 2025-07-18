using System.IO;
using System.Threading.Tasks;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Provides offline sharing of ritual templates via local files. 🧙‍♂️
    /// </summary>
    public interface ITemplateSharingService
    {
        Task ShareTemplateAsync(RitualTemplate template, string path);
    }

    public class TemplateSharingService : ITemplateSharingService
    {
        public async Task ShareTemplateAsync(RitualTemplate template, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await RitualTemplateSerializer.SaveAsync(template, Path.GetDirectoryName(path));
            // Move saved file to destination name
            var saved = Path.Combine(Path.GetDirectoryName(path)!, $"template_{template.TemplateId}.json");
            if (File.Exists(saved))
            {
                File.Move(saved, path, true);
            }
        }
    }
}
