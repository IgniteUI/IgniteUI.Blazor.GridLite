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
    public async Task JSSorting(JsonElement sortExpression)
    {
        if (!GridReference.Sorting.HasDelegate)
            return;

        var expression = sortExpression.Deserialize(GridLiteJsonContext.Default.IgbGridLiteSortingExpression)
            ?? throw new JsonException("sorting event payload deserialized to null");

        var eventArgs = new IgbGridLiteSortingEventArgs
        {
            Expression = expression
        };

        await GridReference.Sorting.InvokeAsync(eventArgs);
    }

    /// <summary>
    /// Callback from JavaScript when a sort operation has completed
    /// </summary>
    /// <param name="sortExpression">The sort expression from JavaScript</param>
    /// <remarks>
    /// Will execute <see cref="IgbGridLite{TItem}.Sorted"/>
    /// </remarks>
    [JSInvokable]
    public async Task JSSorted(JsonElement sortExpression)
    {
        if (!GridReference.Sorted.HasDelegate)
            return;

        var expression = sortExpression.Deserialize(GridLiteJsonContext.Default.IgbGridLiteSortingExpression)
            ?? throw new JsonException("sorted event payload deserialized to null");

        var eventArgs = new IgbGridLiteSortedEventArgs
        {
            Expression = expression
        };

        await GridReference.Sorted.InvokeAsync(eventArgs);
    }

    /// <summary>
    /// Callback from JavaScript when filtering is initiated through the UI
    /// </summary>
    /// <param name="filteringEvent">The filtering event details from JavaScript</param>
    /// <remarks>
    /// Will execute <see cref="IgbGridLite{TItem}.Filtering"/>
    /// </remarks>
    [JSInvokable]
    public async Task JSFiltering(JsonElement filteringEvent)
    {
        if (!GridReference.Filtering.HasDelegate)
            return;

        var eventData = filteringEvent.Deserialize(GridLiteJsonContext.Default.IgbGridLiteFilteringEventArgs)
            ?? throw new JsonException("filtering event payload deserialized to null");

        await GridReference.Filtering.InvokeAsync(eventData);
    }

    /// <summary>
    /// Callback from JavaScript when a filter operation has completed
    /// </summary>
    /// <param name="filteredEvent">The filtered event details from JavaScript</param>
    /// <remarks>
    /// Will execute <see cref="IgbGridLite{TItem}.Filtered"/>
    /// </remarks>
    [JSInvokable]
    public async Task JSFiltered(JsonElement filteredEvent)
    {
        if (!GridReference.Filtered.HasDelegate)
            return;

        var eventData = filteredEvent.Deserialize(GridLiteJsonContext.Default.IgbGridLiteFilteredEventArgs)
            ?? throw new JsonException("filtered event payload deserialized to null");

        await GridReference.Filtered.InvokeAsync(eventData);
    }
}
