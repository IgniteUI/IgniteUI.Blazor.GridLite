using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Event object for the sorting event of the grid.
/// </summary>
public class IgbGridLiteSortingEventArgs
{
    /// <summary>
    /// The sort expression which will be used for the operation.
    /// </summary>
    [JsonPropertyName("expression")]
    public IgbGridLiteSortingExpression Expression { get; set; }

    /// <summary>
    /// Set to true to cancel the operation.
    /// </summary>
    [JsonPropertyName("cancel")]
    internal bool Cancel { get; set; }
}