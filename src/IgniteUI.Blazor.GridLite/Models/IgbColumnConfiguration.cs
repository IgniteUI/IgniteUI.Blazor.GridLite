using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

public class IgbColumnConfiguration
{
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    [JsonPropertyName("dataType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GridLiteColumnDataType? DataType { get; init; }

    [JsonPropertyName("header")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Header { get; init; }

    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Width { get; init; }

    [JsonPropertyName("hidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Hidden { get; init; }

    [JsonPropertyName("resizable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Resizable { get; init; }

    [JsonPropertyName("sortable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Sortable { get; init; }

    [JsonPropertyName("sortingCaseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool SortingCaseSensitive { get; init; }

    [JsonPropertyName("filterable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Filterable { get; init; }

    [JsonPropertyName("filteringCaseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool FilteringCaseSensitive { get; init; }

    [JsonIgnore]
    internal Func<IgbGridLiteHeaderContext<object>, object> HeaderTemplate { get; init; }

    [JsonIgnore]
    internal Func<IgbGridLiteCellContext<object>, object> CellTemplate { get; init; }

    /// <summary>
    /// Converts the column configuration to a JavaScript-compatible format.
    /// Excludes templates and other non-serializable properties.
    /// </summary>
    internal object ToJsConfig()
    {
        return new
        {
            field = Field,
            dataType = DataType?.ToString().ToLower(),
            header = Header,
            width = Width,
            hidden = Hidden,
            resizable = Resizable,
            sortable = Sortable,
            sortingCaseSensitive = SortingCaseSensitive,
            filterable = Filterable,
            filteringCaseSensitive = FilteringCaseSensitive
        };
    }
}

/// <summary>
/// The data type for a column.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<GridLiteColumnDataType>))]
public enum GridLiteColumnDataType
{
    String,
    Number,
    Boolean,
    Date
}