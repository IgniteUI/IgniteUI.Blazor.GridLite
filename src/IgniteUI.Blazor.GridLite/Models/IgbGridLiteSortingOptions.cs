using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Configures the sort behavior for the grid.
/// </summary>
public class IgbGridLiteSortingOptions
{
    /// <summary>
    /// Whether the grid sorts by one column at a time or by several.
    /// </summary>
    [JsonPropertyName("mode")]
    public GridLiteSortingMode Mode { get; set; } = GridLiteSortingMode.Multiple;
}

/// <summary>
/// How many columns the grid sorts by at a time.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<GridLiteSortingMode>))]
public enum GridLiteSortingMode
{
    /// <summary>Sorting by a column adds to the existing sort expressions.</summary>
    Multiple,

    /// <summary>Sorting by a column replaces the existing sort expression.</summary>
    Single,
}
