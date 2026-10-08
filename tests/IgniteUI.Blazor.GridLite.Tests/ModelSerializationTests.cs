using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using IgniteUI.Blazor.Controls;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.GridLite.Tests;

/// <summary>
/// Verifies the wire format of the public models sent through JS interop, through the source-generated
/// metadata the library ships: camelCase property keys and omission of unset optional values.
/// </summary>
public class ModelSerializationTests
{
    private static GridLiteJsonContext Context => GridLiteJsonContext.Default;

    private static JsonElement SerializeToElement<T>(T value, JsonTypeInfo<T> typeInfo)
        => JsonSerializer.SerializeToElement(value, typeInfo);

    [Fact]
    public void SortingExpression_SerializesWithCamelCaseKeys()
    {
        var json = SerializeToElement(new IgbGridLiteSortingExpression
        {
            Key = "ProductName",
            Direction = GridLiteSortingDirection.Descending,
        }, Context.IgbGridLiteSortingExpression);

        Assert.Equal("ProductName", json.GetProperty("key").GetString());
        Assert.Equal("descending", json.GetProperty("direction").GetString());
        Assert.False(json.TryGetProperty("caseSensitive", out _)); // omitted when null
    }

    [Fact]
    public void SortingExpression_IncludesCaseSensitiveWhenSet()
    {
        var json = SerializeToElement(new IgbGridLiteSortingExpression
        {
            Key = "ProductName",
            Direction = GridLiteSortingDirection.Ascending,
            CaseSensitive = true,
        }, Context.IgbGridLiteSortingExpression);

        Assert.True(json.GetProperty("caseSensitive").GetBoolean());
    }

    [Fact]
    public void SortingExpression_RoundTripsFromJsPayload()
    {
        // Shape the JS side sends to the JSSorting/JSSorted callbacks
        const string payload = """{"key":"UnitPrice","direction":"ascending","caseSensitive":false}""";

        var expression = JsonSerializer.Deserialize(payload, Context.IgbGridLiteSortingExpression);

        Assert.NotNull(expression);
        Assert.Equal("UnitPrice", expression.Key);
        Assert.Equal(GridLiteSortingDirection.Ascending, expression.Direction);
        Assert.False(expression.CaseSensitive);
    }

    [Fact]
    public void FilterExpression_SerializesWithCamelCaseKeys_AndPlainStringConditionAndCriteria()
    {
        var json = SerializeToElement(new IgbGridLiteFilterExpression
        {
            Key = "ProductName",
            Condition = "contains",
            SearchTerm = "Cha",
            Criteria = GridLiteFilterCriteria.Or,
            CaseSensitive = false,
        }, Context.IgbGridLiteFilterExpression);

        Assert.Equal("ProductName", json.GetProperty("key").GetString());
        Assert.Equal("\"contains\"", json.GetProperty("condition").GetRawText());
        Assert.Equal("Cha", json.GetProperty("searchTerm").GetString());
        Assert.Equal("\"or\"", json.GetProperty("criteria").GetRawText());
        Assert.False(json.GetProperty("caseSensitive").GetBoolean());
    }

    [Fact]
    public void FilterExpression_OmitsUnsetOptionalValues()
    {
        var json = SerializeToElement(new IgbGridLiteFilterExpression
        {
            Key = "InStock",
            Condition = "true", // unary condition, no search term
        }, Context.IgbGridLiteFilterExpression);

        Assert.Equal("InStock", json.GetProperty("key").GetString());
        Assert.False(json.TryGetProperty("searchTerm", out _));
        Assert.False(json.TryGetProperty("criteria", out _));
        Assert.False(json.TryGetProperty("caseSensitive", out _));
    }

    [Theory]
    [InlineData("\"Ch\"", "Ch")]
    [InlineData("10", 10.0)]
    [InlineData("10.5", 10.5)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void FilterExpression_ReadsSearchTermFromJsPayload_AsPrimitive(string searchTerm, object expected)
    {
        var payload = $$"""{"key":"Price","condition":"greaterThan","searchTerm":{{searchTerm}}}""";

        var expression = JsonSerializer.Deserialize(payload, Context.IgbGridLiteFilterExpression);

        Assert.NotNull(expression);
        Assert.Equal("greaterThan", expression.Condition);
        Assert.Equal(expected, expression.SearchTerm);
        Assert.IsType(expected.GetType(), expression.SearchTerm);
    }

    [Fact]
    public void FilterExpression_ReadsNullOrStructuredSearchTerm()
    {
        var withNull = JsonSerializer.Deserialize("""{"key":"Name","condition":"empty","searchTerm":null}""", Context.IgbGridLiteFilterExpression);
        var withArray = JsonSerializer.Deserialize("""{"key":"Price","condition":"equals","searchTerm":[1,2]}""", Context.IgbGridLiteFilterExpression);

        Assert.Null(withNull!.SearchTerm);
        Assert.Equal(JsonValueKind.Array, Assert.IsType<JsonElement>(withArray!.SearchTerm).ValueKind);
    }

    [Theory]
    [InlineData("add", GridLiteFilteringType.Add)]
    [InlineData("modify", GridLiteFilteringType.Modify)]
    [InlineData("remove", GridLiteFilteringType.Remove)]
    public void FilteringEventArgs_DeserializeFromJsPayload(string type, GridLiteFilteringType expected)
    {
        var payload = $$"""
            {"key":"Name","type":"{{type}}","expressions":[
              {"key":"Name","condition":"contains","searchTerm":"a"},
              {"key":"Name","condition":"startsWith","searchTerm":"b","criteria":"or"}]}
            """;

        var args = JsonSerializer.Deserialize(payload, Context.IgbGridLiteFilteringEventArgs);

        Assert.NotNull(args);
        Assert.Equal(expected, args.Type);
        Assert.Equal(2, args.Expressions.Count);
        Assert.Null(args.Expressions[0].Criteria);
        Assert.Equal("startsWith", args.Expressions[1].Condition);
        Assert.Equal(GridLiteFilterCriteria.Or, args.Expressions[1].Criteria);
    }

    [Fact]
    public void FilteredEventArgs_DeserializeFromJsPayload()
    {
        const string payload = """
            {"key":"Name","state":[{"key":"Name","condition":"contains","searchTerm":"a","criteria":"and"}]}
            """;

        var args = JsonSerializer.Deserialize(payload, Context.IgbGridLiteFilteredEventArgs);

        Assert.NotNull(args);
        var expression = Assert.Single(args.State);
        Assert.Equal("contains", expression.Condition);
        Assert.Equal(GridLiteFilterCriteria.And, expression.Criteria);
    }

    [Theory]
    [InlineData(GridLiteSortingMode.Multiple, "multiple")]
    [InlineData(GridLiteSortingMode.Single, "single")]
    public void SortingOptions_SerializesMode(GridLiteSortingMode mode, string expected)
    {
        var json = SerializeToElement(new IgbGridLiteSortingOptions { Mode = mode }, Context.IgbGridLiteSortingOptions);

        Assert.Equal(expected, json.GetProperty("mode").GetString());
    }

    [Fact]
    public void ColumnConfiguration_SerializesWithCamelCaseKeys_OmittingDefaults()
    {
        var json = SerializeToElement(new IgbGridLiteColumnConfiguration
        {
            Field = "Price",
            DataType = GridLiteColumnDataType.Number,
            Header = "Unit Price",
            Sortable = true,
        }, Context.IgbGridLiteColumnConfiguration);

        Assert.Equal("Price", json.GetProperty("field").GetString());
        Assert.Equal("number", json.GetProperty("dataType").GetString());
        Assert.Equal("Unit Price", json.GetProperty("header").GetString());
        Assert.True(json.GetProperty("sortable").GetBoolean());
        // null strings and false booleans are omitted from the payload
        Assert.False(json.TryGetProperty("width", out _));
        Assert.False(json.TryGetProperty("hidden", out _));
        Assert.False(json.TryGetProperty("resizable", out _));
        Assert.False(json.TryGetProperty("filterable", out _));
    }

    [Fact]
    public void ColumnConfiguration_DeserializesFromJsPayload()
    {
        // Shape returned by the JS side for getColumns
        const string payload = """
            {"field":"ProductName","dataType":"string","header":"Product Name","width":"120px",
             "hidden":false,"resizable":true,"sortable":true,"sortingCaseSensitive":false,
             "filterable":true,"filteringCaseSensitive":false}
            """;

        var column = JsonSerializer.Deserialize(payload, Context.IgbGridLiteColumnConfiguration);

        Assert.NotNull(column);
        Assert.Equal("ProductName", column.Field);
        Assert.Equal(GridLiteColumnDataType.String, column.DataType);
        Assert.Equal("Product Name", column.Header);
        Assert.Equal("120px", column.Width);
        Assert.False(column.Hidden);
        Assert.True(column.Resizable);
        Assert.True(column.Sortable);
        Assert.True(column.Filterable);
    }
}
