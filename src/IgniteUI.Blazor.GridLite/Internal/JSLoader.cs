using Microsoft.JSInterop;

namespace IgniteUI.Blazor.Controls.Internal;

internal static class JSLoader
{
    public static async Task<IJSObjectReference> LoadAsync(
        IJSRuntime jsRuntime,
        string path = null)
    {
        var javascriptPath = path ??
            "./_content/IgniteUI.Blazor.GridLite/js/blazor-igc-grid-lite.js";

        var module = await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", javascriptPath);

        return await module.InvokeAsync<IJSObjectReference>(
            "get_igc_grid_lite");
    }
}