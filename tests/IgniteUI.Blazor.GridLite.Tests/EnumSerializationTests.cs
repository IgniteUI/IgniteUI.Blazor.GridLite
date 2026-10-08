using System.Text.Json;
using IgniteUI.Blazor.Controls;

namespace IgniteUI.Blazor.GridLite.Tests;

/// <summary>
/// Verifies the camelCase string contract for enums crossing the JS interop
/// boundary (via the internal CamelCaseEnumConverter applied per enum):
/// - values serialize to camelCase strings
/// - camelCase strings round-trip back to the enum value
/// - raw integers are rejected on deserialize
/// </summary>
public class EnumSerializationTests
{
    [Theory]
    [InlineData(GridLiteSortingDirection.Ascending, "ascending")]
    [InlineData(GridLiteSortingDirection.Descending, "descending")]
    [InlineData(GridLiteSortingDirection.None, "none")]
    public void SortingDirection_SerializesToCamelCase(GridLiteSortingDirection value, string expected)
    {
        Assert.Equal($"\"{expected}\"", JsonSerializer.Serialize(value));
        Assert.Equal(value, JsonSerializer.Deserialize<GridLiteSortingDirection>($"\"{expected}\""));
    }

    [Theory]
    [InlineData(GridLiteColumnDataType.String, "string")]
    [InlineData(GridLiteColumnDataType.Number, "number")]
    [InlineData(GridLiteColumnDataType.Boolean, "boolean")]
    public void ColumnDataType_SerializesToCamelCase(GridLiteColumnDataType value, string expected)
    {
        Assert.Equal($"\"{expected}\"", JsonSerializer.Serialize(value));
        Assert.Equal(value, JsonSerializer.Deserialize<GridLiteColumnDataType>($"\"{expected}\""));
    }

    [Theory]
    [InlineData(GridLiteSortingMode.Multiple, "multiple")]
    [InlineData(GridLiteSortingMode.Single, "single")]
    public void SortingMode_SerializesToCamelCase(GridLiteSortingMode value, string expected)
    {
        Assert.Equal($"\"{expected}\"", JsonSerializer.Serialize(value));
        Assert.Equal(value, JsonSerializer.Deserialize<GridLiteSortingMode>($"\"{expected}\""));
    }

    // Without the converter, the criteria reach grid-lite as numbers, which it does not read as 'and' / 'or'.
    [Theory]
    [InlineData(GridLiteFilterCriteria.And, "and")]
    [InlineData(GridLiteFilterCriteria.Or, "or")]
    public void FilterCriteria_SerializesToCamelCase(GridLiteFilterCriteria value, string expected)
    {
        Assert.Equal($"\"{expected}\"", JsonSerializer.Serialize(value));
        Assert.Equal(value, JsonSerializer.Deserialize<GridLiteFilterCriteria>($"\"{expected}\""));
    }

    // Without the converter, grid-lite's 'add' / 'modify' / 'remove' fail to deserialize and Filtering never runs.
    [Theory]
    [InlineData(GridLiteFilteringType.Add, "add")]
    [InlineData(GridLiteFilteringType.Modify, "modify")]
    [InlineData(GridLiteFilteringType.Remove, "remove")]
    public void FilteringType_SerializesToCamelCase(GridLiteFilteringType value, string expected)
    {
        Assert.Equal($"\"{expected}\"", JsonSerializer.Serialize(value));
        Assert.Equal(value, JsonSerializer.Deserialize<GridLiteFilteringType>($"\"{expected}\""));
    }

    [Fact]
    public void Enums_RejectIntegerValues()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<GridLiteSortingDirection>("0"));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<GridLiteColumnDataType>("1"));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<GridLiteSortingMode>("0"));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<GridLiteFilterCriteria>("1"));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<GridLiteFilteringType>("0"));
    }

    [Fact]
    public void Enums_RejectUnknownStrings()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<GridLiteSortingDirection>("\"sideways\""));
    }

    [Fact]
    public void Enums_RejectUndefinedValuesOnSerialize()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Serialize((GridLiteSortingDirection)999));
    }
}
