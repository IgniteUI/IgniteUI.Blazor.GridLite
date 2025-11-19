namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Context object for the column header template callback.
/// </summary>
/// <typeparam name="TItem">The data type for grid rows</typeparam>
internal class IgbGridLiteHeaderContext<TItem> where TItem : class
{
    /// <summary>
    /// The current configuration for the column.
    /// </summary>
    public ColumnConfiguration Column { get; set; }
}