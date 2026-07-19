using Bunit;
using IgniteUI.Blazor.Controls;

namespace IgniteUI.Blazor.GridLite.Tests;

/// <summary>
/// Sample data item used by the grid tests.
/// </summary>
public class TestItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
}

/// <summary>
/// Base class for IgbGridLite component tests.
/// Stubs the JS module chain the component loads on first render
/// (module import -> <c>get_igc_grid_lite</c>) so tests can verify every
/// call that goes through JS interop, including its serialized payloads.
/// The component's <c>IJSRuntime</c> injection is the seam — bUnit replaces
/// it with <see cref="Bunit.TestContext.JSInterop"/>, no library changes needed.
/// </summary>
public abstract class GridLiteTestBase : TestContext
{
    protected const string ModulePath = "./_content/IgniteUI.Blazor.GridLite/js/blazor-igc-grid-lite.js";

    protected static readonly IReadOnlyList<TestItem> Items =
    [
        new() { Id = 1, Name = "Chai", Price = 18.0 },
        new() { Id = 2, Name = "Chang", Price = 19.0 },
        new() { Id = 3, Name = "Aniseed Syrup", Price = 10.0 },
    ];

    /// <summary>The stub for the imported blazor-igc-grid-lite.js module.</summary>
    protected BunitJSModuleInterop Module { get; }

    /// <summary>
    /// The stub for the grid API object returned by <c>get_igc_grid_lite()</c>.
    /// All <c>blazor_igc_grid_lite.*</c> invocations are recorded here.
    /// </summary>
    protected BunitJSModuleInterop GridApi { get; }

    protected GridLiteTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Module = JSInterop.SetupModule(ModulePath);
        GridApi = Module.SetupModule("get_igc_grid_lite", Array.Empty<object>());
    }

    /// <summary>
    /// Renders an <see cref="IgbGridLite{TItem}"/> with sample data and waits for
    /// JS initialization to complete (the initial <c>renderGrid</c> interop call).
    /// </summary>
    protected IRenderedComponent<IgbGridLite<TestItem>> RenderGrid(
        Action<ComponentParameterCollectionBuilder<IgbGridLite<TestItem>>>? configure = null)
    {
        var cut = RenderComponent<IgbGridLite<TestItem>>(ps =>
        {
            ps.Add(x => x.Data, Items);
            configure?.Invoke(ps);
        });

        cut.WaitForAssertion(() => GridApi.VerifyInvoke("blazor_igc_grid_lite.renderGrid"));
        return cut;
    }
}
