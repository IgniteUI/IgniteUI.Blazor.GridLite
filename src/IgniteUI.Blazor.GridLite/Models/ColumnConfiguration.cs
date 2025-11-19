using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

public class ColumnConfiguration
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DataType? Type { get; set; }

    [JsonPropertyName("headerText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string HeaderText { get; set; }

    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Width { get; set; }

    [JsonPropertyName("hidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Hidden { get; set; }

    [JsonPropertyName("resizable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Resizable { get; set; }

    [JsonPropertyName("sort")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Sort { get; set; }

    [JsonPropertyName("filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Filter { get; set; }

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
            key = Key,
            type = Type?.ToString().ToLower(),
            headerText = HeaderText,
            width = Width,
            hidden = Hidden,
            resizable = Resizable,
            sort = ConvertSortConfig(Sort),
            filter = ConvertFilterConfig(Filter)
        };
    }

    private static object ConvertSortConfig(object sort)
    {
        if (sort == null) return null;
        if (sort is bool b) return b;
        if (sort is ColumnSortConfiguration config)
        {
            return new
            {
                caseSensitive = config.CaseSensitive
                // Note: Comparer functions cannot be serialized
            };
        }
        return sort;
    }

    private static object ConvertFilterConfig(object filter)
    {
        if (filter == null) return null;
        if (filter is bool b) return b;
        if (filter is ColumnFilterConfiguration config)
        {
            return new
            {
                caseSensitive = config.CaseSensitive
            };
        }
        return filter;
    }
}

/// <summary>
/// The data type for a column.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<DataType>))]
public enum DataType
{
    String,
    Number,
    Boolean,
    Date
}