using IgniteUI.Blazor.Controls.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// IgbGridLite is a component for displaying data in a tabular format quick and easy.
/// </summary>
/// <typeparam name="TItem">The data type of the items to display in the grid</typeparam>
public partial class IgbGridLite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TItem>
    : ComponentBase, IAsyncDisposable where TItem : class
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// The data to display in the grid
    /// </summary>
    /// <remarks>
    /// Serialized with reflection, keeping the C# property names. With full trimming the public properties of
    /// <typeparamref name="TItem"/> are kept; complex types nested in it must be preserved by the app.
    /// </remarks>
    [Parameter]
    public IEnumerable<TItem>? Data { get; set; }

    /// <summary>
    /// Child content for declarative column definitions
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the grid will try to "resolve" its column configuration based on the passed data source.
    /// This is usually executed on initial rendering in the DOM.
    /// </summary>
    [Parameter]
    public bool AutoGenerate { get; set; } = false;

    /// <summary>
    /// Whether the grid will adopt document-level styles into its shadow DOM.
    /// Useful when using cell and header templates that rely on styles defined at the document level.
    /// </summary>
    [Parameter]
    public bool AdoptRootStyles { get; set; } = false;

    /// <summary>
    /// Sort options property for the grid.
    /// </summary>
    [Parameter]
    public IgbGridLiteSortingOptions? SortingOptions { get; set; }

    /// <summary>
    /// Initial sort expressions to apply when the grid is rendered
    /// </summary>
    [Parameter]
    public IEnumerable<IgbGridLiteSortingExpression>? SortingExpressions { get; set; }

    /// <summary>
    /// Initial filter expressions to apply when the grid is rendered
    /// </summary>
    [Parameter]
    public IEnumerable<IgbGridLiteFilterExpression>? FilterExpressions { get; set; }

    /// <summary>
    /// Additional attributes for the component's HTML element
    /// element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Fires when sorting is initiated through the UI.
    /// Returns the sort expression which will be used for the operation.
    /// </summary>
    [Parameter]
    public EventCallback<IgbGridLiteSortingEventArgs> Sorting { get; set; }

    /// <summary>
    /// Fires when a sort operation initiated through the UI has completed.
    /// Returns the sort expression used for the operation.
    /// </summary>
    [Parameter]
    public EventCallback<IgbGridLiteSortedEventArgs> Sorted { get; set; }

    /// <summary>
    /// Fires when filtering is initiated through the UI.
    /// </summary>
    [Parameter]
    public EventCallback<IgbGridLiteFilteringEventArgs> Filtering { get; set; }

    /// <summary>
    /// Fires when a filter operation initiated through the UI has completed.
    /// Returns the filter state for the affected column.
    /// </summary>
    [Parameter]
    public EventCallback<IgbGridLiteFilteredEventArgs> Filtered { get; set; }

    /// <summary>
    /// Fires once, when the grid has rendered on the client for the first time.
    /// </summary>
    [Parameter]
    public EventCallback Rendered { get; set; }

    private ElementReference grid;
    private IJSObjectReference? blazorIgbGridLite;
    private JSHandler<TItem>? jsHandler;
    private readonly string gridId = Guid.NewGuid().ToString("N");
    private GridLiteEventFlags? sentEvents;
    private bool disposed;

    // True once the grid has rendered on the client; false if the first render fails or disposal comes first.
    private readonly TaskCompletionSource<bool> clientRender = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// The unique identifier for this grid instance
    /// </summary>
    [Obsolete("The grid's internal id is not needed to use it. It will be removed in a future release.")]
    public string GridId => gridId;

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        try
        {
            blazorIgbGridLite = await JSLoader.LoadAsync(JSRuntime);

            if (disposed)
            {
                // Disposal ran while the script was loading, before there was anything to release.
                await DisposeAsync();
                return;
            }

            jsHandler = new JSHandler<TItem>(this);
            await RenderGridAsync();
        }
        finally
        {
            // After a failed first render, calls waiting for it return instead of hanging.
            clientRender.TrySetResult(false);
        }
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        GridLiteUpdateConfig? updateConfig = null;

        if (jsHandler is not null)
        {
            if (parameters.TryGetValue<IEnumerable<TItem>?>(nameof(Data), out var newData)
                && !ReferenceEquals(Data, newData))
            {
                (updateConfig ??= new()).Data = CreateDataPayload(newData);
            }

            if (parameters.TryGetValue<bool>(nameof(AutoGenerate), out var newAutoGenerate)
                && AutoGenerate != newAutoGenerate)
            {
                (updateConfig ??= new()).AutoGenerate = newAutoGenerate;
            }

            if (parameters.TryGetValue<bool>(nameof(AdoptRootStyles), out var newAdoptRootStyles)
                && AdoptRootStyles != newAdoptRootStyles)
            {
                (updateConfig ??= new()).AdoptRootStyles = newAdoptRootStyles;
            }

            // The web component cannot take null for these; a reset to null restores its default instead.
            if (parameters.TryGetValue<IgbGridLiteSortingOptions?>(nameof(SortingOptions), out var newSortOptions)
                && !ReferenceEquals(SortingOptions, newSortOptions))
            {
                (updateConfig ??= new()).SortingOptions = newSortOptions ?? new();
            }

            if (parameters.TryGetValue<IEnumerable<IgbGridLiteSortingExpression>?>(nameof(SortingExpressions), out var newSortingExpressions)
                && !ReferenceEquals(SortingExpressions, newSortingExpressions))
            {
                (updateConfig ??= new()).SortingExpressions = newSortingExpressions ?? [];
            }

            if (parameters.TryGetValue<IEnumerable<IgbGridLiteFilterExpression>?>(nameof(FilterExpressions), out var newFilterExpressions)
                && !ReferenceEquals(FilterExpressions, newFilterExpressions))
            {
                (updateConfig ??= new()).FilterExpressions = newFilterExpressions ?? [];
            }
        }

        await base.SetParametersAsync(parameters);

        // The client listens only for bound callbacks, so binding or unbinding one re-attaches its listeners.
        var events = CreateEventFlags();
        if (jsHandler is not null && events != sentEvents)
        {
            sentEvents = events;
            (updateConfig ??= new()).Events = events;
        }

        if (updateConfig != null)
        {
            var json = JsonSerializer.Serialize(updateConfig, GridLiteJsonContext.Default.GridLiteUpdateConfig);
            await InvokeVoidJsAsync("blazor_igc_grid_lite.updateGrid", gridId, json);
        }
    }

    /// <summary>
    /// Re-renders the grid on the client with the current data and configuration.
    /// </summary>
    [Obsolete("The grid renders on its own and updates from its parameters; assign a new collection instead of changing one in place. It will be removed in a future release.")]
    public async Task RenderAsync()
    {
        await RenderGridAsync();
    }

    private async Task RenderGridAsync()
    {
        if (jsHandler is null)
            return;

        await Task.Yield();

        // A disposal during the yield has sent destroyGrid already; rendering now would leave the grid registered on the client.
        if (disposed)
            return;

        sentEvents = CreateEventFlags();

        // TODO: expose the web component's dataPipelineConfiguration (remote sort/filter hooks). Its hooks are
        // client-side callbacks, so they need a JS-to-.NET round trip and/or a value serialized here.
        var config = new GridLiteRenderConfig
        {
            Id = gridId,
            Data = CreateDataPayload(Data),
            AutoGenerate = AutoGenerate,
            AdoptRootStyles = AdoptRootStyles,
            SortingOptions = SortingOptions,
            SortingExpressions = SortingExpressions,
            FilterExpressions = FilterExpressions,
            Events = sentEvents,
        };

        var json = JsonSerializer.Serialize(config, GridLiteJsonContext.Default.GridLiteRenderConfig);

        await InvokeVoidJsAsync("blazor_igc_grid_lite.renderGrid", jsHandler.ObjectReference, grid, json);

        if (clientRender.TrySetResult(true))
        {
            await Rendered.InvokeAsync();
        }
    }

    private GridLiteEventFlags CreateEventFlags() => new()
    {
        HasSorting = Sorting.HasDelegate,
        HasSorted = Sorted.HasDelegate,
        HasFiltering = Filtering.HasDelegate,
        HasFiltered = Filtered.HasDelegate,
    };

    private static GridLiteDataPayload CreateDataPayload(IEnumerable<TItem>? data)
    {
        // The web component spreads `data`, so it cannot take null; a null parameter means "no rows".
        return new GridLiteDataPayload(data ?? Array.Empty<TItem>(), AppValueSerializer.GetDataTypeInfo<TItem>());
    }

    /// <summary>
    /// Refreshes the grid by re-rendering it with the current data and configuration.
    /// </summary>
    [Obsolete("The grid renders on its own and updates from its parameters; assign a new collection instead of changing one in place. It will be removed in a future release.")]
    public async Task RefreshAsync()
    {
        await RenderGridAsync();
    }

    /// <summary>
    /// Updates the data source for the grid.
    /// </summary>
    /// <param name="newData">The new data to display in the grid</param>
    [Obsolete("Assign a new collection to the Data parameter instead. It will be removed in a future release.")]
    public async Task UpdateDataAsync(IEnumerable<TItem> newData)
    {
        ArgumentNullException.ThrowIfNull(newData);
        Data = newData;
        var json = JsonSerializer.Serialize(newData, AppValueSerializer.GetDataTypeInfo<TItem>());
        await InvokeVoidJsAsync("blazor_igc_grid_lite.updateData", gridId, json);
    }

    /// <summary>
    /// Performs a sort operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expression">The sort expression to apply</param>
    public async Task SortAsync(IgbGridLiteSortingExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var json = JsonSerializer.Serialize(expression, GridLiteJsonContext.Default.IgbGridLiteSortingExpression);
        await InvokeGridAsync("blazor_igc_grid_lite.sort", gridId, json);
    }

    /// <summary>
    /// Performs a sort operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The sort expression(s) to apply</param>
    public async Task SortAsync(IEnumerable<IgbGridLiteSortingExpression> expressions)
    {
        ArgumentNullException.ThrowIfNull(expressions);
        var json = JsonSerializer.Serialize(expressions, GridLiteJsonContext.Default.IEnumerableIgbGridLiteSortingExpression);
        await InvokeGridAsync("blazor_igc_grid_lite.sort", gridId, json);
    }

    /// <summary>
    /// Resets the current sort state of the grid.
    /// </summary>
    /// <param name="key">Optional column field. If provided, only clears sort for that column.
    /// If null, clears all sorting.</param>
    public async Task ClearSortAsync(string? key = null)
    {
        await InvokeGridAsync("blazor_igc_grid_lite.clearSort", gridId, key);
    }

    /// <summary>
    /// Performs a filter operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expression">The filter expression to apply</param>
    public async Task FilterAsync(IgbGridLiteFilterExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var json = JsonSerializer.Serialize(expression, GridLiteJsonContext.Default.IgbGridLiteFilterExpression);
        await InvokeGridAsync("blazor_igc_grid_lite.filter", gridId, json);
    }

    /// <summary>
    /// Performs a filter operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The filter expression(s) to apply</param>
    public async Task FilterAsync(IEnumerable<IgbGridLiteFilterExpression> expressions)
    {
        ArgumentNullException.ThrowIfNull(expressions);
        var json = JsonSerializer.Serialize(expressions, GridLiteJsonContext.Default.IEnumerableIgbGridLiteFilterExpression);
        await InvokeGridAsync("blazor_igc_grid_lite.filter", gridId, json);
    }

    /// <summary>
    /// Resets the current filter state of the grid.
    /// </summary>
    /// <param name="key">Optional column field. If provided, only clears filter for that column.
    /// If null, clears all filtering.</param>
    public async Task ClearFilterAsync(string? key = null)
    {
        await InvokeGridAsync("blazor_igc_grid_lite.clearFilter", gridId, key);
    }

    /// <summary>
    /// Returns the current column configuration list.
    /// </summary>
    /// <returns>The column configurations</returns>
    public async Task<IgbGridLiteColumnConfiguration[]> GetColumnsAsync()
    {
        if (!await clientRender.Task)
        {
            return [];
        }

        var columns = await InvokeJsAsync("blazor_igc_grid_lite.getColumns", gridId);
        return columns is { ValueKind: JsonValueKind.Array } array
            ? array.Deserialize(GridLiteJsonContext.Default.IgbGridLiteColumnConfigurationArray) ?? []
            : [];
    }

    /// <summary>
    /// Navigates to a position in the grid based on provided row index and column field.
    /// </summary>
    /// <param name="row">The row index to navigate to</param>
    /// <param name="field">The column field to navigate to, if any</param>
    /// <param name="activate">Optionally also activate the navigated cell</param>
    public async Task NavigateToAsync(int row, string? field = null, bool activate = false)
    {
        await InvokeGridAsync("blazor_igc_grid_lite.navigateTo", gridId, row, field, activate);
    }

    // Before the first client render there is no grid on the client for the call to reach, so it waits for one.
    private async ValueTask InvokeGridAsync(string identifier, params object?[] args)
    {
        if (await clientRender.Task)
        {
            await InvokeVoidJsAsync(identifier, args);
        }
    }

    // Results come back as JsonElement so the caller deserializes them through GridLiteJsonContext.
    private async ValueTask<JsonElement?> InvokeJsAsync(string identifier, params object?[] args)
    {
        if (blazorIgbGridLite == null)
        {
            return null;
        }

        try
        {
            return await blazorIgbGridLite.InvokeAsync<JsonElement>(identifier, args);
        }
        catch (Exception ex) when (ex is ObjectDisposedException ||
                                  ex is JSDisconnectedException)
        {
            return null;
        }
    }

    private async ValueTask InvokeVoidJsAsync(string identifier, params object?[] args)
    {
        if (blazorIgbGridLite == null)
        {
            return;
        }

        try
        {
            if (blazorIgbGridLite is IJSInProcessObjectReference jsInProcessRuntime)
            {
                jsInProcessRuntime.InvokeVoid(identifier, args);
            }
            else
            {
                await blazorIgbGridLite.InvokeVoidAsync(identifier, args);
            }
        }
        catch (Exception ex) when (ex is ObjectDisposedException ||
                                  ex is JSDisconnectedException)
        { }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        disposed = true;
        clientRender.TrySetResult(false);

        try
        {
            if (blazorIgbGridLite != null)
            {
                await InvokeVoidJsAsync("blazor_igc_grid_lite.destroyGrid", gridId);
                await blazorIgbGridLite.DisposeAsync();
            }
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is JSDisconnectedException)
        { }
        finally
        {
            // The client's reference to it keeps this component alive, so it is released even when a call above fails.
            jsHandler?.Dispose();
        }
    }
}
