using Microsoft.JSInterop;
using System.Text.Json;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// Provides internal-only <see cref="JSInvokableAttribute"/> callbacks for IgbGridLite
/// </summary>
/// <typeparam name="TItem">The data type of the items to display in the grid</typeparam>
internal sealed class JSHandler<TItem> : IDisposable where TItem : class
{
    private readonly IgbGridLite<TItem> GridReference;
    internal readonly DotNetObjectReference<JSHandler<TItem>> ObjectReference;

    internal JSHandler(IgbGridLite<TItem> gridReference)
    {
        ObjectReference = DotNetObjectReference.Create(this);
        GridReference = gridReference;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ObjectReference?.Dispose();
    }

    /// <summary>
    /// Callback from JavaScript when sorting is initiated through the UI
    /// </summary>
    /// <param name="sortExpression">The sort expression from JavaScript</param>
    /// <remarks>
    /// Will execute <see cref="IgbGridLite{TItem}.OnSorting"/>
    /// </remarks>
    [JSInvokable]
    public async Task<bool> JSSorting(JsonElement sortExpression)
    {
        if (!GridReference.OnSorting.HasDelegate)
            return false; // Don't cancel

        try
        {
            var expression = JsonSerializer.Deserialize<SortExpression>(
                sortExpression.GetRawText());

            var eventArgs = new IgbGridLiteSortingEvent
            {
                Expression = expression,
                Cancel = false
            };

            await GridReference.OnSorting.InvokeAsync(eventArgs);

            // Return true to cancel the operation
            return eventArgs.Cancel;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Callback from JavaScript when a sort operation has completed
    /// </summary>
    /// <param name="sortExpression">The sort expression from JavaScript</param>
    /// <remarks>
    /// Will execute <see cref="IgbGridLite{TItem}.OnSorted"/>
    /// </remarks>
    [JSInvokable]
    public void JSSorted(JsonElement sortExpression)
    {
        if (!GridReference.OnSorted.HasDelegate)
            return;

        try
        {
            var expression = JsonSerializer.Deserialize<SortExpression>(
                sortExpression.GetRawText());

            var eventArgs = new IgbGridLiteSortedEvent
            {
                Expression = expression
            };

            GridReference.OnSorted.InvokeAsync(eventArgs);
        }
        catch
        {
            // Ignore deserialization errors
        }
    }

    /// <summary>
    /// Callback from JavaScript when filtering is initiated through the UI
    /// </summary>
    /// <param name="filteringEvent">The filtering event details from JavaScript</param>
    /// <remarks>
    /// Will execute <see cref="IgbGridLite{TItem}.OnFiltering"/>
    /// </remarks>
    [JSInvokable]
    public async Task<bool> JSFiltering(JsonElement filteringEvent)
    {
        if (!GridReference.OnFiltering.HasDelegate)
            return false; // Don't cancel

        try
        {
            var eventData = JsonSerializer.Deserialize<IgbGridLiteFilteringEvent>(
                filteringEvent.GetRawText());

            await GridReference.OnFiltering.InvokeAsync(eventData);

            // IgbGridLiteFilteringEvent doesn't have a Cancel property in the TypeScript definition
            // but you could add it if needed
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Callback from JavaScript when a filter operation has completed
    /// </summary>
    /// <param name="filteredEvent">The filtered event details from JavaScript</param>
    /// <remarks>
    /// Will execute <see cref="IgbGridLite{TItem}.OnFiltered"/>
    /// </remarks>
    [JSInvokable]
    public void JSFiltered(JsonElement filteredEvent)
    {
        if (!GridReference.OnFiltered.HasDelegate)
            return;

        try
        {
            var eventData = JsonSerializer.Deserialize<IgbGridLiteFilteredEvent>(
                filteredEvent.GetRawText());

            GridReference.OnFiltered.InvokeAsync(eventData);
        }
        catch
        {
            // Ignore deserialization errors
        }
    }

    /// <summary>
    /// Callback from JavaScript when a cell is clicked
    /// </summary>
    /// <param name="cellData">The cell data from JavaScript</param>
    [JSInvokable]
    public void JSCellClick(JsonElement cellData)
    {
        // Add cell click handling if needed
        // This would require adding an OnCellClick event to IgbGridLite
    }

    /// <summary>
    /// Callback from JavaScript when a row is clicked
    /// </summary>
    /// <param name="rowData">The row data from JavaScript</param>
    [JSInvokable]
    public void JSRowClick(JsonElement rowData)
    {
        // Add row click handling if needed
        // This would require adding an OnRowClick event to IgbGridLite
    }

    /// <summary>
    /// Callback from JavaScript when the grid data view changes
    /// </summary>
    /// <param name="dataView">The current data view from JavaScript</param>
    [JSInvokable]
    public void JSDataViewChanged(JsonElement dataView)
    {
        // Handle data view changes if needed
        // This could be used to track the current visible/filtered data
    }
}