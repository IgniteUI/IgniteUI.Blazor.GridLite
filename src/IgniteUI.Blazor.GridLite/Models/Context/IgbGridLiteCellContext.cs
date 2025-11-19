namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Context object for the row cell template callback.
/// </summary>
/// <typeparam name="TItem">The data type for grid rows</typeparam>
internal class IgbGridLiteCellContext<TItem> where TItem : class
{
    /// <summary>
    /// The current configuration for the column.
    /// </summary>
    public ColumnConfiguration Column { get; set; }

    /// <summary>
    /// The value from the data source for this cell.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// The index of the current row.
    /// </summary>
    public int RowIndex { get; set; }

    /// <summary>
    /// The data item for the current row.
    /// </summary>
    public TItem Data { get; set; }
}