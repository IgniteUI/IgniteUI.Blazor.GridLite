using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// Provides internal-only <see cref="JSInvokableAttribute"/> callbacks for IgbGridLite
/// </summary>
/// <typeparam name="TItem">The data type of the items to display in the grid</typeparam>
internal sealed class JSHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TItem>
    : IDisposable where TItem : class
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
    /// Will execute <see cref="IgbGridLite{TItem}.Sorting"/>
    /// </remarks>
    [JSInvokable]
    public async Task<bool> JSSorting(JsonElement sortExpression)
    {
        if (!GridReference.Sorting.HasDelegate)
            return false; // Don't cancel

        try
        {
            var expression = sortExpression.Deserialize(GridLiteJsonContext.Default.IgbGridLiteSortingExpression)
                ?? throw new JsonException("sorting event payload deserialized to null");

            var eventArgs = new IgbGridLiteSortingEventArgs
            {
                Expression = expression,
                Cancel = false
            };

            await GridReference.Sorting.InvokeAsync(eventArgs);

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
    /// Will execute <see cref="IgbGridLite{TItem}.Sorted"/>
    /// </remarks>
    [JSInvokable]
    public void JSSorted(JsonElement sortExpression)
    {
        if (!GridReference.Sorted.HasDelegate)
            return;

        try
        {
            var expression = sortExpression.Deserialize(GridLiteJsonContext.Default.IgbGridLiteSortingExpression)
                ?? throw new JsonException("sorted event payload deserialized to null");

            var eventArgs = new IgbGridLiteSortedEventArgs
            {
                Expression = expression
            };

            GridReference.Sorted.InvokeAsync(eventArgs);
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
    /// Will execute <see cref="IgbGridLite{TItem}.Filtering"/>
    /// </remarks>
    [JSInvokable]
    public async Task<bool> JSFiltering(JsonElement filteringEvent)
    {
        if (!GridReference.Filtering.HasDelegate)
            return false; // Don't cancel

        try
        {
            var eventData = filteringEvent.Deserialize(GridLiteJsonContext.Default.IgbGridLiteFilteringEventArgs)
                ?? throw new JsonException("filtering event payload deserialized to null");

            await GridReference.Filtering.InvokeAsync(eventData);

            // IgbGridLiteFilteringEventArgs doesn't have a Cancel property in the TypeScript definition
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
    /// Will execute <see cref="IgbGridLite{TItem}.Filtered"/>
    /// </remarks>
    [JSInvokable]
    public void JSFiltered(JsonElement filteredEvent)
    {
        if (!GridReference.Filtered.HasDelegate)
            return;

        try
        {
            var eventData = filteredEvent.Deserialize(GridLiteJsonContext.Default.IgbGridLiteFilteredEventArgs)
                ?? throw new JsonException("filtered event payload deserialized to null");

            GridReference.Filtered.InvokeAsync(eventData);
        }
        catch
        {
            // Ignore deserialization errors
        }
    }
}
