using Bunit;
using IgniteUI.Blazor.Controls;
using IgniteUI.Blazor.Controls.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace IgniteUI.Blazor.GridLite.Tests;

public class JSLoaderTests : GridLiteTestBase
{
    private sealed class FailingRuntime(Exception exception) : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            ValueTask.FromException<TValue>(exception);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            ValueTask.FromException<TValue>(exception);
    }

    [Fact]
    public async Task LoadAsync_WhenDisconnected_ReturnsNull()
    {
        var runtime = new FailingRuntime(new JSDisconnectedException("disconnected"));

        Assert.Null(await JSLoader.LoadAsync(runtime));
    }

    [Fact]
    public async Task LoadAsync_WhenImportFails_PropagatesError()
    {
        var runtime = new FailingRuntime(new JSException("module failed to load"));

        await Assert.ThrowsAsync<JSException>(() => JSLoader.LoadAsync(runtime));
    }

    [Fact]
    public async Task Grid_WhenImportDisconnects_DoesNotWaitForClientRender()
    {
        Services.AddSingleton<IJSRuntime>(new FailingRuntime(new JSDisconnectedException("disconnected")));

        var cut = Render<IgbGridLite<TestItem>>(ps => ps.Add(x => x.Data, Items));

        await cut.InvokeAsync(() => cut.Instance.SortAsync(new IgbGridLiteSortingExpression { Key = "Name" }))
            .WaitAsync(TimeSpan.FromSeconds(5));
    }
}
