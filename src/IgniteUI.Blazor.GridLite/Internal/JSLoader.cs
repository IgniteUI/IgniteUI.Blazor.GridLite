using Microsoft.JSInterop;

namespace IgniteUI.Blazor.Controls.Internal;

internal static class JSLoader
{
    private const string ModulePath = "./_content/IgniteUI.Blazor.GridLite/js/blazor-igc-grid-lite.js";

    public static async Task<IJSObjectReference> LoadAsync(IJSRuntime jsRuntime)
    {
        var module = await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", ModulePath);

        return await module.InvokeAsync<IJSObjectReference>(
            "get_igc_grid_lite");
    }
}
