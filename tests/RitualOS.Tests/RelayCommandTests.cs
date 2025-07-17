using RitualOS.Helpers;

namespace RitualOS.Tests;

public class RelayCommandTests
{
    [Fact]
    public void Execute_InvokesAction()
    {
        bool executed = false;
        var command = new RelayCommand(_ => executed = true);

        command.Execute(null);

        Assert.True(executed);
    }
}
