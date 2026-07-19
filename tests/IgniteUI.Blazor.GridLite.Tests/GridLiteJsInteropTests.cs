using System.Text.Json;
using Bunit;
using IgniteUI.Blazor.Controls;

namespace IgniteUI.Blazor.GridLite.Tests;

/// <summary>
/// Verifies that the public API of <see cref="IgbGridLite{TItem}"/> drives the
/// JS interop layer correctly: right identifiers, right grid id, and correctly
/// serialized payloads. The JS side itself is stubbed by bUnit's JSInterop, so
/// these tests validate everything up to the JS boundary.
/// </summary>
public class GridLiteJsInteropTests : GridLiteTestBase
{
    private const string Api = "blazor_igc_grid_lite";

    private static JsonElement ParseJsonArgument(object? argument)
    {
        using var doc = JsonDocument.Parse(Assert.IsType<string>(argument));
        return doc.RootElement.Clone();
    }

    [Fact]
    public void InitialRender_InvokesRenderGrid_WithFullConfig()
    {
        var cut = RenderGrid();

        var invocation = GridApi.VerifyInvoke($"{Api}.renderGrid");
        Assert.Equal(4, invocation.Arguments.Count);
        Assert.NotNull(invocation.Arguments[0]); // DotNetObjectReference for JS -> .NET callbacks

        var config = ParseJsonArgument(invocation.Arguments[2]);
        Assert.Equal(cut.Instance.GridId, config.GetProperty("id").GetString());
        Assert.Equal(Items.Count, config.GetProperty("data").GetArrayLength());
        Assert.False(config.GetProperty("autoGenerate").GetBoolean());
        Assert.False(config.GetProperty("adoptRootStyles").GetBoolean());
    }

    [Fact]
    public void InitialRender_SerializesInitialState_InConfig()
    {
        RenderGrid(ps => ps
            .Add(x => x.SortingOptions, new IgbGridLiteSortingOptions { Mode = GridLiteSortingMode.Single })
            .Add(x => x.SortingExpressions,
            [
                new IgbGridLiteSortingExpression { Key = "Name", Direction = GridLiteSortingDirection.Descending },
            ])
            .Add(x => x.FilterExpressions,
            [
                new IgbGridLiteFilterExpression { Key = "Name", Condition = "contains", SearchTerm = "a" },
            ]));

        var config = ParseJsonArgument(GridApi.VerifyInvoke($"{Api}.renderGrid").Arguments[2]);

        Assert.Equal("single", config.GetProperty("sortingOptions").GetProperty("mode").GetString());

        var sort = config.GetProperty("sortingExpressions")[0];
        Assert.Equal("Name", sort.GetProperty("key").GetString());
        Assert.Equal("descending", sort.GetProperty("direction").GetString());

        var filter = config.GetProperty("filterExpressions")[0];
        Assert.Equal("contains", filter.GetProperty("condition").GetString());
        Assert.Equal("a", filter.GetProperty("searchTerm").GetString());
    }

    [Fact]
    public void InitialRender_PassesEventFlags_ForBoundCallbacks()
    {
        RenderGrid(ps => ps
            .Add(x => x.Sorting, (IgbGridLiteSortingEventArgs _) => { })
            .Add(x => x.Filtered, (IgbGridLiteFilteredEventArgs _) => { }));

        var flagsArgument = GridApi.VerifyInvoke($"{Api}.renderGrid").Arguments[3];
        Assert.NotNull(flagsArgument);
        var flags = ParseJsonArgument(JsonSerializer.Serialize(flagsArgument));

        Assert.True(flags.GetProperty("hasSorting").GetBoolean());
        Assert.False(flags.GetProperty("hasSorted").GetBoolean());
        Assert.False(flags.GetProperty("hasFiltering").GetBoolean());
        Assert.True(flags.GetProperty("hasFiltered").GetBoolean());
    }

    [Fact]
    public void InitialRender_FiresRenderedCallback()
    {
        var renderedCount = 0;

        var cut = RenderGrid(ps => ps.Add(x => x.Rendered, () => renderedCount++));

        cut.WaitForAssertion(() => Assert.Equal(1, renderedCount));
    }

    [Fact]
    public async Task RefreshAsync_InvokesRenderGridAgain()
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.RefreshAsync());

        GridApi.VerifyInvoke($"{Api}.renderGrid", calledTimes: 2);
    }

    [Fact]
    public async Task UpdateDataAsync_InvokesUpdateData_WithSerializedData()
    {
        var cut = RenderGrid();
        var newData = new List<TestItem> { new() { Id = 42, Name = "Ipoh Coffee", Price = 46.0 } };

        await cut.InvokeAsync(() => cut.Instance.UpdateDataAsync(newData));

        var invocation = GridApi.VerifyInvoke($"{Api}.updateData");
        Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
        var data = ParseJsonArgument(invocation.Arguments[1]);
        Assert.Equal(1, data.GetArrayLength());
        Assert.Same(newData, cut.Instance.Data);
    }

    [Fact]
    public async Task SortAsync_SingleExpression_InvokesSort_WithCamelCasePayload()
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.SortAsync(new IgbGridLiteSortingExpression
        {
            Key = "Name",
            Direction = GridLiteSortingDirection.Descending,
        }));

        var invocation = GridApi.VerifyInvoke($"{Api}.sort");
        Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
        var expression = ParseJsonArgument(invocation.Arguments[1]);
        Assert.Equal("Name", expression.GetProperty("key").GetString());
        Assert.Equal("descending", expression.GetProperty("direction").GetString());
    }

    [Fact]
    public async Task SortAsync_ExpressionList_InvokesSort_WithArrayPayload()
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.SortAsync(
        [
            new IgbGridLiteSortingExpression { Key = "Name", Direction = GridLiteSortingDirection.Ascending },
            new IgbGridLiteSortingExpression { Key = "Price", Direction = GridLiteSortingDirection.Descending },
        ]));

        var expressions = ParseJsonArgument(GridApi.VerifyInvoke($"{Api}.sort").Arguments[1]);
        Assert.Equal(2, expressions.GetArrayLength());
        Assert.Equal("ascending", expressions[0].GetProperty("direction").GetString());
        Assert.Equal("Price", expressions[1].GetProperty("key").GetString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Name")]
    public async Task ClearSortAsync_InvokesClearSort_WithOptionalKey(string? key)
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.ClearSortAsync(key));

        var invocation = GridApi.VerifyInvoke($"{Api}.clearSort");
        Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
        Assert.Equal(key, invocation.Arguments[1]);
    }

    [Fact]
    public async Task FilterAsync_SingleExpression_InvokesFilter_WithCamelCasePayload()
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.FilterAsync(new IgbGridLiteFilterExpression
        {
            Key = "Name",
            Condition = "contains",
            SearchTerm = "Cha",
        }));

        var invocation = GridApi.VerifyInvoke($"{Api}.filter");
        Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
        var expression = ParseJsonArgument(invocation.Arguments[1]);
        Assert.Equal("Name", expression.GetProperty("key").GetString());
        Assert.Equal("contains", expression.GetProperty("condition").GetString());
        Assert.Equal("Cha", expression.GetProperty("searchTerm").GetString());
    }

    [Fact]
    public async Task FilterAsync_ExpressionList_InvokesFilter_WithArrayPayload()
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.FilterAsync(
        [
            new IgbGridLiteFilterExpression { Key = "Name", Condition = "contains", SearchTerm = "a" },
            new IgbGridLiteFilterExpression { Key = "Name", Condition = "contains", SearchTerm = "b", Criteria = "or" },
        ]));

        var expressions = ParseJsonArgument(GridApi.VerifyInvoke($"{Api}.filter").Arguments[1]);
        Assert.Equal(2, expressions.GetArrayLength());
        Assert.Equal("or", expressions[1].GetProperty("criteria").GetString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Name")]
    public async Task ClearFilterAsync_InvokesClearFilter_WithOptionalKey(string? key)
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.ClearFilterAsync(key));

        var invocation = GridApi.VerifyInvoke($"{Api}.clearFilter");
        Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
        Assert.Equal(key, invocation.Arguments[1]);
    }

    [Fact]
    public async Task NavigateToAsync_InvokesNavigateTo_WithAllArguments()
    {
        var cut = RenderGrid();

        await cut.InvokeAsync(() => cut.Instance.NavigateToAsync(5, "Price", activate: true));

        var invocation = GridApi.VerifyInvoke($"{Api}.navigateTo");
        Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
        Assert.Equal(5L, invocation.Arguments[1]);
        Assert.Equal("Price", invocation.Arguments[2]);
        Assert.True(Assert.IsType<bool>(invocation.Arguments[3]));
    }

    [Fact]
    public async Task GetColumnsAsync_ReturnsColumns_FromJsResult()
    {
        var cut = RenderGrid();
        var expected = new[]
        {
            new IgbColumnConfiguration { Field = "Name", Sortable = true },
            new IgbColumnConfiguration { Field = "Price", DataType = GridLiteColumnDataType.Number },
        };
        GridApi.Setup<IgbColumnConfiguration[]>($"{Api}.getColumns", _ => true).SetResult(expected);

        var columns = await cut.InvokeAsync(() => cut.Instance.GetColumnsAsync().AsTask());

        var invocation = GridApi.VerifyInvoke($"{Api}.getColumns");
        Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
        Assert.Same(expected, columns);
    }

    [Fact]
    public void ChangingData_InvokesUpdateGrid_WithOnlyChangedKeys()
    {
        var cut = RenderGrid();
        var newData = new List<TestItem> { new() { Id = 9, Name = "Tofu", Price = 23.25 } };

        cut.SetParametersAndRender(ps => ps.Add(x => x.Data, newData));

        cut.WaitForAssertion(() =>
        {
            var invocation = GridApi.VerifyInvoke($"{Api}.updateGrid");
            Assert.Equal(cut.Instance.GridId, invocation.Arguments[0]);
            var update = ParseJsonArgument(invocation.Arguments[1]);
            Assert.Single(update.EnumerateObject());
            Assert.Equal(1, update.GetProperty("data").GetArrayLength());
        });
    }

    [Fact]
    public void ChangingMultipleParameters_InvokesUpdateGrid_WithAllChangedKeys()
    {
        var cut = RenderGrid();

        cut.SetParametersAndRender(ps => ps
            .Add(x => x.AutoGenerate, true)
            .Add(x => x.SortingOptions, new IgbGridLiteSortingOptions { Mode = GridLiteSortingMode.Single }));

        cut.WaitForAssertion(() =>
        {
            var update = ParseJsonArgument(GridApi.VerifyInvoke($"{Api}.updateGrid").Arguments[1]);
            Assert.True(update.GetProperty("autoGenerate").GetBoolean());
            Assert.Equal("single", update.GetProperty("sortingOptions").GetProperty("mode").GetString());
            Assert.False(update.TryGetProperty("data", out _)); // unchanged parameter not resent
        });
    }

    [Fact]
    public void UnchangedParameters_DoNotInvokeUpdateGrid()
    {
        var cut = RenderGrid();

        cut.SetParametersAndRender(ps => ps.Add(x => x.Data, Items)); // same reference

        GridApi.VerifyNotInvoke($"{Api}.updateGrid");
        GridApi.VerifyInvoke($"{Api}.renderGrid", calledTimes: 1); // no re-render either
    }

    [Fact]
    public void Dispose_InvokesDestroyGrid()
    {
        var cut = RenderGrid();
        var gridId = cut.Instance.GridId;

        DisposeComponents();

        cut.WaitForAssertion(() =>
        {
            var invocation = GridApi.VerifyInvoke($"{Api}.destroyGrid");
            Assert.Equal(gridId, invocation.Arguments[0]);
        });
    }
}
