using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Event object for the sorting event of the grid.
/// </summary>
public class IgbGridLiteSortingEvent
{
    /// <summary>
    /// The sort expression which will be used for the operation.
    /// </summary>
    [JsonPropertyName("expression")]
    public SortExpression Expression { get; set; }

    /// <summary>
    /// Set to true to cancel the operation.
    /// </summary>
    [JsonPropertyName("cancel")]
    public bool Cancel { get; set; }
}