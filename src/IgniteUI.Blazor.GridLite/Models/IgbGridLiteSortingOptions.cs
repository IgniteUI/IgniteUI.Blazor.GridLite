using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Configures the sort behavior for the grid.
/// </summary>
public class IgbGridLiteSortingOptions
{   
    /// <summary>   
    /// Sorting mode - "single" or "multiple".
    /// </summary>
    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "multiple";
}