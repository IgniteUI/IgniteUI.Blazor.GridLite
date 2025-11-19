using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Event object for the filtering event of the grid.
/// </summary>
public class IgbGridLiteFilteringEvent
{
    /// <summary>
    /// The target column for the filter operation.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// The filter expression(s) to apply.
    /// </summary>
    [JsonPropertyName("expressions")]
    public List<IgbGridLiteFilterExpression> Expressions { get; set; }

    /// <summary>
    /// The type of modification which will be applied to the filter state of the column.
    /// 'add' - a new filter expression will be added to the state of the column.
    /// 'modify' - an existing filter expression will be modified.
    /// 'remove' - the expression(s) will be removed from the state of the column.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } // "add", "modify", or "remove"
}