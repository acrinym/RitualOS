using Avalonia;
using Avalonia.Headless;
using RitualOS;

namespace RitualOS.Tests;

public class ViewTests
{
    [Fact]
    public void ChakraSelector_LoadsHeadless()
    {
        AppBuilder.Configure<App>()
            .UseHeadless()
            .SetupWithoutStarting();

        var view = new RitualOS.Views.ChakraSelector();
        Assert.NotNull(view);
    }
}
