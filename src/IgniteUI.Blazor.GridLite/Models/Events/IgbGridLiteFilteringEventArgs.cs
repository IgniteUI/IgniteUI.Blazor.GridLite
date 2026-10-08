using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Event object for the filtering event of the grid.
/// </summary>
public class IgbGridLiteFilteringEventArgs
{
    /// <summary>
    /// The target column for the filter operation.
    /// </summary>
    [JsonPropertyName("key")]
    public required string Key { get; init; }

    /// <summary>
    /// The filter expression(s) to apply.
    /// </summary>
    [JsonPropertyName("expressions")]
    public required IReadOnlyList<IgbGridLiteFilterExpression> Expressions { get; init; }

    /// <summary>
    /// The type of modification which will be applied to the filter state of the column.
    /// </summary>
    [JsonPropertyName("type")]
    public required GridLiteFilteringType Type { get; init; }
}

/// <summary>
/// The type of modification a filter operation applies to the filter state of a column.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<GridLiteFilteringType>))]
public enum GridLiteFilteringType
{
    /// <summary>A new filter expression will be added to the state of the column.</summary>
    Add,

    /// <summary>An existing filter expression will be modified.</summary>
    Modify,

    /// <summary>The expression(s) will be removed from the state of the column.</summary>
    Remove,
}
