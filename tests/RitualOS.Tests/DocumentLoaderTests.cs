using System.IO;
using System.Threading.Tasks;
using RitualOS.Services;

namespace RitualOS.Tests;

public class DocumentLoaderTests
{
    [Fact]
    public async Task LoadAsync_ReturnsSameContentAsSync()
    {
        var temp = Path.GetTempFileName();
        await File.WriteAllTextAsync(temp, "hello world");
        try
        {
            var syncDoc = DocumentLoader.Load(temp);
            var asyncDoc = await DocumentLoader.LoadAsync(temp);
            Assert.Equal(syncDoc.Content, asyncDoc.Content);
        }
        finally
        {
            File.Delete(temp);
        }
    }
}
