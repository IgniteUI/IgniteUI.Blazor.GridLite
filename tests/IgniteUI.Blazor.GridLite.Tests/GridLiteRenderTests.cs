using Bunit;
using IgniteUI.Blazor.Controls;

namespace IgniteUI.Blazor.GridLite.Tests;

/// <summary>
/// bUnit rendering tests for <see cref="IgbGridLite{TItem}"/> and
/// <see cref="IgbGridLiteColumn"/> markup output.
/// </summary>
public class GridLiteRenderTests : GridLiteTestBase
{
    [Fact]
    public void Grid_RendersRootElement_WithGeneratedId()
    {
        var cut = RenderGrid();

        var element = cut.Find("igc-grid-lite");
        Assert.False(string.IsNullOrEmpty(cut.Instance.GridId));
        Assert.Equal(cut.Instance.GridId, element.GetAttribute("id"));
    }

    [Fact]
    public void Grid_LoadsJsModule_OnFirstRender()
    {
        RenderGrid();

        var import = JSInterop.VerifyInvoke("import");
        Assert.Equal(ModulePath, import.Arguments[0]);
    }

    [Fact]
    public void Grid_SplatsAdditionalAttributes()
    {
        var cut = RenderGrid(ps => ps
            .AddUnmatched("class", "my-grid")
            .AddUnmatched("data-testid", "grid-under-test"));

        var element = cut.Find("igc-grid-lite");
        Assert.Equal("my-grid", element.GetAttribute("class"));
        Assert.Equal("grid-under-test", element.GetAttribute("data-testid"));
    }

    [Fact]
    public void Grid_RendersDeclarativeColumns_WithMappedAttributes()
    {
        var cut = RenderGrid(ps => ps
            .AddChildContent<IgbGridLiteColumn>(c => c
                .Add(x => x.Field, nameof(TestItem.Name))
                .Add(x => x.Header, "Product Name")
                .Add(x => x.Sortable, true)
                .Add(x => x.Filterable, true))
            .AddChildContent<IgbGridLiteColumn>(c => c
                .Add(x => x.Field, nameof(TestItem.Price))
                .Add(x => x.DataType, GridLiteColumnDataType.Number)
                .Add(x => x.Width, "120px")
                .Add(x => x.Resizable, true)));

        var columns = cut.FindAll("igc-grid-lite-column");
        Assert.Equal(2, columns.Count);

        Assert.Equal("Name", columns[0].GetAttribute("field"));
        Assert.Equal("Product Name", columns[0].GetAttribute("header"));
        Assert.Equal("true", columns[0].GetAttribute("sortable"));
        Assert.Equal("true", columns[0].GetAttribute("filterable"));
        Assert.False(columns[0].HasAttribute("data-type"));
        Assert.False(columns[0].HasAttribute("resizable"));

        Assert.Equal("Price", columns[1].GetAttribute("field"));
        Assert.Equal("number", columns[1].GetAttribute("data-type"));
        Assert.Equal("120px", columns[1].GetAttribute("width"));
        Assert.Equal("true", columns[1].GetAttribute("resizable"));
        Assert.False(columns[1].HasAttribute("sortable"));
    }

    [Fact]
    public void Column_OmitsAttributes_ForDefaultValues()
    {
        var cut = RenderGrid(ps => ps
            .AddChildContent<IgbGridLiteColumn>(c => c
                .Add(x => x.Field, nameof(TestItem.Id))));

        var column = cut.Find("igc-grid-lite-column");
        Assert.Equal("Id", column.GetAttribute("field"));
        foreach (var attribute in new[]
        {
            "hidden", "resizable", "sortable", "filterable",
            "sorting-case-sensitive", "filtering-case-sensitive", "data-type",
        })
        {
            Assert.False(column.HasAttribute(attribute), $"expected no '{attribute}' attribute");
        }
    }

    [Fact]
    public void GridId_IsStable_AcrossParameterUpdates()
    {
        var cut = RenderGrid();
        var initialId = cut.Instance.GridId;

        cut.SetParametersAndRender(ps => ps.Add(x => x.AutoGenerate, true));

        Assert.Equal(initialId, cut.Instance.GridId);
        Assert.Equal(initialId, cut.Find("igc-grid-lite").GetAttribute("id"));
    }

    [Fact]
    public void Grid_DisposesCleanly()
    {
        RenderGrid();

        DisposeComponents(); // must not throw
    }
}
