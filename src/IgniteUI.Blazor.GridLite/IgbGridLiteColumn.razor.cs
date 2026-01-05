using Microsoft.AspNetCore.Components;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Column component for IgbGridLite. Used declaratively to define columns.
/// </summary>
public partial class IgbGridLiteColumn : ComponentBase
{
    /// <summary>
    /// The field from the data that the column references.
    /// </summary>
    [Parameter]
    public string? Field { get; set; }

    /// <summary>
    /// The header text of the column.
    /// </summary>
    [Parameter]
    public string? Header { get; set; }

    /// <summary>
    /// The data type of the column's values.
    /// </summary>
    [Parameter]
    public GridLiteColumnDataType? DataType { get; set; }

    /// <summary>
    /// The width of the column (CSS value).
    /// </summary>
    [Parameter]
    public string? Width { get; set; }

    /// <summary>
    /// Whether the column is hidden.
    /// </summary>
    [Parameter]
    public bool Hidden { get; set; }

    /// <summary>
    /// Whether the column is resizable.
    /// </summary>
    [Parameter]
    public bool Resizable { get; set; }

    /// <summary>
    /// Whether the column is sortable.
    /// </summary>
    [Parameter]
    public bool Sortable { get; set; }

    /// <summary>
    /// Whether sorting should be case sensitive for this column.
    /// </summary>
    [Parameter]
    public bool? SortingCaseSensitive { get; set; }

    /// <summary>
    /// Whether the column is filterable.
    /// </summary>
    [Parameter]
    public bool Filterable { get; set; }

    /// <summary>
    /// Whether filtering should be case sensitive for this column.
    /// </summary>
    [Parameter]
    public bool? FilteringCaseSensitive { get; set; }
}
