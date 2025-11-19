using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Event object for the filtered event of the grid.
/// </summary>
public class IgbGridLiteFilteredEvent
{
    /// <summary>
    /// The target column for the filter operation.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// The filter state of the column after the operation.
    /// </summary>
    [JsonPropertyName("state")]
    public List<IgbGridLiteFilterExpression> State { get; set; }
}