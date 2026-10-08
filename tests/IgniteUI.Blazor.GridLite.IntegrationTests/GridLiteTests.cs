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

    // TODO: grid-lite 0.11.0's client mapping a condition object to its name, remove after it's updated.
    [Test]
    public async Task FilteringEvent_WithConditionObject_ReachesDotNetAsItsName()
    {
        await Page.GotoAsync(Host.ServerAddress);
        var grid = Page.Locator("igc-grid-lite");
        await Expect(grid.GetByText("Chai", new() { Exact = true })).ToBeVisibleAsync();

        await grid.EvaluateAsync(
            @"grid => grid.dispatchEvent(new CustomEvent('filtering', {
                  detail: {
                      key: 'ProductName',
                      expressions: [{
                          key: 'ProductName',
                          condition: { name: 'contains', label: 'Contains', unary: false },
                          searchTerm: 'Ch'
                      }],
                      type: 'add'
                  }
              }))");

        await Expect(Page.Locator("#last-filtering")).ToHaveTextAsync("Add contains");
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

    // Without the options form, activate is dropped when no field is given: grid-lite reads the null column
    // argument of its deprecated positional overload as an options object.
    [Test]
    public async Task NavigateToAsync_WithoutField_ActivatesCellInRow()
    {
        await Page.GotoAsync(Host.ServerAddress);
        var grid = Page.Locator("igc-grid-lite");
        await Expect(grid.GetByText("Chai", new() { Exact = true })).ToBeVisibleAsync();

        await Page.Locator("#navigate-button").ClickAsync();

        // The active state is a cell property, not an attribute, so it is polled.
        var cells = grid.Locator("igc-grid-lite-cell");
        var activeRows = "none";
        for (var attempt = 0; attempt < 50 && activeRows == "none"; attempt++)
        {
            activeRows = await cells.EvaluateAllAsync<string>(
                "cells => cells.filter(c => c.active).map(c => c.row.index).join(',') || 'none'");
            if (activeRows == "none")
            {
                await Task.Delay(100);
            }
        }

        Assert.That(activeRows, Is.EqualTo("3"), "Row indices of the active cells.");
    }
}
