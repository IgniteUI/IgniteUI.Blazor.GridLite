using IgniteUI.Blazor.GridLite.IntegrationTests.Infrastructure;

namespace IgniteUI.Blazor.GridLite.IntegrationTests;

public class GridLiteTests : BlazorPageTest<Program>
{
    [Test]
    public async Task GridLite_RendersHeadersAndData()
    {
        await Page.GotoAsync(Host.ServerAddress);

        var grid = Page.Locator("igc-grid-lite");
        await grid.WaitForAsync();

        // Playwright locators pierce the shadow DOM, so header and cell
        // content rendered by the web component are directly reachable.
        await Expect(grid.GetByText("Product Name")).ToBeVisibleAsync();
        await Expect(grid.GetByText("Chai", new() { Exact = true })).ToBeVisibleAsync();
    }
}
