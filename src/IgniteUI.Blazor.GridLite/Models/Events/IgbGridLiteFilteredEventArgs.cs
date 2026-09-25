using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Event object for the filtered event of the grid.
/// </summary>
public class IgbGridLiteFilteredEventArgs
{
    /// <summary>
    /// The target column for the filter operation.
    /// </summary>
    [JsonPropertyName("key")]
    public required string Key { get; set; }

    /// <summary>
    /// The filter state of the column after the operation.
    /// </summary>
    [JsonPropertyName("state")]
    public required List<IgbGridLiteFilterExpression> State { get; set; }
}
