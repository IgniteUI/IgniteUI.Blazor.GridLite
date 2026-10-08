using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// A column's configuration as the grid reports it, returned by <see cref="IgbGridLite{TItem}.GetColumnsAsync"/>.
/// </summary>
public class IgbGridLiteColumnConfiguration
{
    /// <summary>
    /// The field from the data that the column references.
    /// </summary>
    [JsonPropertyName("field")]
    public required string Field { get; init; }

    /// <summary>
    /// The data type of the column's values; the grid treats a column without one as <see cref="GridLiteColumnDataType.String"/>.
    /// </summary>
    [JsonPropertyName("dataType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GridLiteColumnDataType? DataType { get; init; }

    /// <summary>
    /// The header text of the column; the grid shows the field when it is not set.
    /// </summary>
    [JsonPropertyName("header")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Header { get; init; }

    /// <summary>
    /// The width of the column (CSS value); when not set, the column shares the grid's width with the other columns.
    /// </summary>
    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Width { get; init; }

    /// <summary>
    /// Whether the column is hidden.
    /// </summary>
    [JsonPropertyName("hidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Hidden { get; init; }

    /// <summary>
    /// Whether the column is resizable.
    /// </summary>
    [JsonPropertyName("resizable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Resizable { get; init; }

    /// <summary>
    /// Whether the column is sortable.
    /// </summary>
    [JsonPropertyName("sortable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Sortable { get; init; }

    /// <summary>
    /// Whether sorting is case sensitive for this column.
    /// </summary>
    [JsonPropertyName("sortingCaseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool SortingCaseSensitive { get; init; }

    /// <summary>
    /// Whether the column is filterable.
    /// </summary>
    [JsonPropertyName("filterable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Filterable { get; init; }

    /// <summary>
    /// Whether filtering is case sensitive for this column.
    /// </summary>
    [JsonPropertyName("filteringCaseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool FilteringCaseSensitive { get; init; }
}

/// <summary>
/// The data type for a column.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<GridLiteColumnDataType>))]
public enum GridLiteColumnDataType
{
    /// <summary>Text values.</summary>
    String,

    /// <summary>Numeric values.</summary>
    Number,

    /// <summary><see langword="true"/>/<see langword="false"/> values.</summary>
    Boolean,
}
