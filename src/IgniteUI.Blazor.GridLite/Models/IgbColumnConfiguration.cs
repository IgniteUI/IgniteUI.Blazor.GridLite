using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

public class IgbColumnConfiguration
{
    [JsonPropertyName("field")]
    public string Field { get; set; }

    [JsonPropertyName("dataType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GridLiteColumnDataType? DataType { get; set; }

    [JsonPropertyName("header")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Header { get; set; }

    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Width { get; set; }

    [JsonPropertyName("hidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Hidden { get; set; }

    [JsonPropertyName("resizable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Resizable { get; set; }

    [JsonPropertyName("sortable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Sortable { get; set; }

    [JsonPropertyName("sortingCaseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SortingCaseSensitive { get; set; }

    [JsonPropertyName("filterable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Filterable { get; set; }

    [JsonPropertyName("filteringCaseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? FilteringCaseSensitive { get; set; }

    [JsonIgnore]
    internal Func<IgbGridLiteHeaderContext<object>, object> HeaderTemplate { get; set; }

    [JsonIgnore]
    internal Func<IgbGridLiteCellContext<object>, object> CellTemplate { get; set; }

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