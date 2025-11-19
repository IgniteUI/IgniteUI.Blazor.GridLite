using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Event object for the sorted event of the grid.
/// </summary>
public class IgbGridLiteSortedEvent
{
    /// <summary>
    /// The sort expression used for the operation.
    /// </summary>
    [JsonPropertyName("expression")]
    public IgbGridLiteSortExpression Expression { get; set; }
}