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

    // Without it, each re-render adds another set of listeners and every event reaches .NET once more.
    [Test]
    public async Task Refresh_DoesNotDuplicateEventCallbacks()
    {
        await Page.GotoAsync(Host.ServerAddress);
        var grid = Page.Locator("igc-grid-lite");
        await Expect(grid.GetByText("Chai", new() { Exact = true })).ToBeVisibleAsync();

        await Page.Locator("#refresh-button").ClickAsync();
        await Expect(Page.Locator("#refresh-count")).ToHaveTextAsync("1");

        // grid-lite sorts from the header's sort action, not from its title.
        await grid.Locator("igc-grid-lite-header", new() { HasText = "Product Name" }).Locator("[part~='action']").ClickAsync();
        await Expect(Page.Locator("#sorted-count")).ToHaveTextAsync("1");

        // The circuit handles this click after any duplicate callback the sort click sent before it.
        await Page.Locator("#refresh-button").ClickAsync();
        await Expect(Page.Locator("#refresh-count")).ToHaveTextAsync("2");
        await Expect(Page.Locator("#sorted-count")).ToHaveTextAsync("1");
    }

    // Without it, a callback bound after the first render has no listener on the client and never fires.
    [Test]
    public async Task CallbackBoundAfterRender_Fires()
    {
        await Page.GotoAsync(Host.ServerAddress);
        var grid = Page.Locator("igc-grid-lite");
        await Expect(grid.GetByText("Chai", new() { Exact = true })).ToBeVisibleAsync();

        await Page.Locator("#bind-filtered-button").ClickAsync();
        // The grid's updateGrid call reaches the client before the render batch that shows this.
        await Expect(Page.Locator("#filtered-bound")).ToHaveTextAsync("True");

        // Driving the filter row adds nothing here; the event is dispatched in the shape grid-lite sends.
        await grid.EvaluateAsync(
            @"grid => grid.dispatchEvent(new CustomEvent('filtered', {
                  detail: { key: 'ProductName', state: [] }
              }))");

        await Expect(Page.Locator("#filtered-count")).ToHaveTextAsync("1");
    }
}
