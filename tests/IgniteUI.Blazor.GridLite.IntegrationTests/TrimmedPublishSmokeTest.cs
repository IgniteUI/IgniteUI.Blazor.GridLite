using System.Collections.Concurrent;
using IgniteUI.Blazor.GridLite.IntegrationTests.Infrastructure;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace IgniteUI.Blazor.GridLite.IntegrationTests;

/// <summary>
/// Browser checks over the PublishSmoke app's trimmed publish output — the runtime gate for
/// behavior the trim analyzer cannot verify: suppression justifications and annotations that only
/// hold if ILLink honors them, which trim silently when they stop holding. Mirrors the checklist in
/// <c>tests/IgniteUI.Blazor.GridLite.PublishSmoke/README.md</c>.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[Category("TrimmedPublish")]
public class TrimmedPublishSmokeTest : BrowserTest
{
    private static TrimmedPublishServer server = null!;

    // Filled from Playwright's dispatcher thread while the test thread may be reading.
    private IBrowserContext context = null!;
    private IPage page = null!;
    private ILocator grid = null!;
    private ConcurrentQueue<string> pageErrors = null!;
    private ConcurrentQueue<string> failedRequests = null!;

    [OneTimeSetUp]
    public static async Task StartServer() => server = await TrimmedPublishServer.StartAsync();

    [OneTimeTearDown]
    public static async Task StopServer()
    {
        if (server is not null)
        {
            await server.DisposeAsync();
        }
    }

    [SetUp]
    public async Task LoadApp()
    {
        pageErrors = new();
        failedRequests = new();

        context = await NewContext(new BrowserNewContextOptions { BaseURL = server.BaseUrl });
        page = await context.NewPageAsync();
        page.Console += (_, message) =>
        {
            // Resource-load failures land in failedRequests with the URL (and the favicon
            // exclusion); the console duplicate carries neither.
            if (message.Type == "error" && !message.Text.StartsWith("Failed to load resource"))
            {
                pageErrors.Enqueue(message.Text);
            }
        };
        page.PageError += (_, error) => pageErrors.Enqueue(error);
        page.Response += (_, response) =>
        {
            if (response.Status >= 400 && !response.Url.EndsWith("favicon.ico"))
            {
                failedRequests.Enqueue($"{response.Status} {response.Url}");
            }
        };

        await page.GotoAsync("/", new() { WaitUntil = WaitUntilState.NetworkIdle });
        grid = page.Locator("igc-grid-lite");
        // The rows land through interop after the element upgrades.
        await page.WaitForFunctionAsync("() => document.querySelector('igc-grid-lite')?.dataView?.length === 3",
            null, new() { Timeout = 30000 });
    }

    [TearDown]
    public async Task CloseContext()
    {
        if (context is not null)
        {
            await context.CloseAsync();
        }
    }

    /// <summary>
    /// The item type carries no attribute, so its values reach the grid only if ILLink honors the
    /// grid's annotation on <c>TItem</c>; the nested type's values only if it honors the app's
    /// <c>DynamicDependency</c>. Either failing renders empty cells, with no error.
    /// </summary>
    [Test]
    public async Task Data_KeepsItemAndNestedTypeProperties()
    {
        await Expect(grid.GetByText("Beta", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(grid.GetByText("12.5", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(grid.GetByText("B-2", new() { Exact = true })).ToBeVisibleAsync();
    }

    /// <summary>
    /// A header sort click raises the grid's <c>sorted</c> event, whose detail must deserialize through the
    /// source-generated context and reach the app's callback through the kept <c>[JSInvokable]</c> handler.
    /// </summary>
    [Test]
    public async Task HeaderSortClick_RaisesSortedCallback()
    {
        // grid-lite sorts from the header's sort action, not from its title.
        await grid.Locator("igc-grid-lite-header", new() { HasText = "Name" }).Locator("[part~='action']").ClickAsync();

        await Expect(page.Locator("#sorted-result")).ToHaveTextAsync("Name Ascending");
    }

    /// <summary>
    /// The numeric search term is written by its runtime type through the reflection-based serializer;
    /// a value it cannot write fails the call, and a wrong shape filters nothing.
    /// </summary>
    [Test]
    public async Task FilterAsync_WithNumericSearchTerm_FiltersRows()
    {
        await page.Locator("#filter-button").ClickAsync();

        await page.WaitForFunctionAsync("() => document.querySelector('igc-grid-lite').dataView.length === 2");
    }

    /// <summary>
    /// The filter row raises <c>filtered</c> with grid-lite's operand object as the condition; dispatched
    /// directly here in that shape, since driving the filter row adds nothing trim-specific.
    /// </summary>
    [Test]
    public async Task FilteredEvent_RaisesFilteredCallback()
    {
        await grid.EvaluateAsync(
            @"grid => grid.dispatchEvent(new CustomEvent('filtered', {
                  detail: { key: 'Price', state: [{ key: 'Price', condition: { name: 'greaterThan' }, searchTerm: 10 }] }
              }))");

        await Expect(page.Locator("#filtered-result")).ToHaveTextAsync("Price: 1 expression(s)");
    }

    [Test]
    public void CleanLoad_NoErrorsOrFailedRequests()
    {
        Assert.Multiple(() =>
        {
            Assert.That(pageErrors, Is.Empty, "Console/page errors during the trimmed app load.");
            Assert.That(failedRequests, Is.Empty, "Failed requests during the trimmed app load.");
        });
    }
}
