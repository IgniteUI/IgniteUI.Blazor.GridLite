using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Extended sort configuration for a column.
/// </summary>
public class IgbColumnSortConfiguration
{
    /// <summary>
    /// Whether the sort operations will be case sensitive.
    /// </summary>
    [JsonPropertyName("caseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CaseSensitive { get; set; }

    /// <summary>
    /// Custom comparer function for sort operations for this column.
    /// Note: This is not directly supported in Blazor and would need JavaScript interop.
    /// </summary>
    [JsonIgnore]
    internal Func<object, object, int> Comparer { get; set; }
}