using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Extended filter configuration for a column.
/// </summary>
public class IgbColumnFilterConfiguration
{
    /// <summary>
    /// Whether the filter operations will be case sensitive.
    /// </summary>
    [JsonPropertyName("caseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CaseSensitive { get; set; }
}