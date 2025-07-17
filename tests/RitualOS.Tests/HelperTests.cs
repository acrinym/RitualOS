using RitualOS.Helpers;
using RitualOS.Models;

namespace RitualOS.Tests;

public class HelperTests
{
    [Fact]
    public void ChakraHelper_ReturnsExpectedEmoji()
    {
        Assert.Equal("\uD83D\uDD34", ChakraHelper.GetEmoji(Chakra.Root));
        Assert.Equal("\uD83D\uDFE0", ChakraHelper.GetEmoji(Chakra.Sacral));
        Assert.Equal("\uD83D\uDFE1", ChakraHelper.GetEmoji(Chakra.SolarPlexus));
        Assert.Equal("\uD83D\uDC9A", ChakraHelper.GetEmoji(Chakra.Heart));
        Assert.Equal("\uD83D\uDD35", ChakraHelper.GetEmoji(Chakra.Throat));
        Assert.Equal("\uD83D\uDD39", ChakraHelper.GetEmoji(Chakra.ThirdEye));
        Assert.Equal("\u26AA", ChakraHelper.GetEmoji(Chakra.Crown));
    }

    [Fact]
    public void ElementHelper_DisplayName_MatchesEnum()
    {
        Assert.Equal("Water", ElementHelper.GetDisplayName(Element.Water));
    }
}
