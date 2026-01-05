using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Configures the sort behavior for the grid.
/// </summary>
public class IgbGridLiteSortingOptions
{   
    /// <summary>   
    /// The sorting mode. Can be "single" or "multiple".
    /// </summary>
    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "multiple";
}