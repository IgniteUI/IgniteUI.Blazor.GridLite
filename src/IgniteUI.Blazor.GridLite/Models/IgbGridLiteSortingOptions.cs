using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

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
    public GridLiteSortingMode Mode { get; set; } = GridLiteSortingMode.Multiple;
}

/// <summary>
/// The data type for a column.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<GridLiteSortingMode>))]
public enum GridLiteSortingMode
{
    Multiple,
    Single,
}