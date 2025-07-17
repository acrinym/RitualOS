using RitualOS.Services;
using Xunit;

namespace RitualOS.Tests;

public class DreamParserTests
{
    [Fact]
    public async Task AnalyzeDream_ReturnsSymbolMeanings()
    {
        var service = new DreamParserService();
        var analysis = await service.AnalyzeDreamAsync("fire and water");
        Assert.Contains("fire", analysis.ExtractedSymbols);
        Assert.Contains("water", analysis.ExtractedSymbols);
        Assert.True(analysis.SymbolMeanings.ContainsKey("fire"));
        Assert.True(analysis.SymbolMeanings.ContainsKey("water"));
    }
}
