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
    : ComponentBase, IDisposable where TItem : class
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
    /// The options to customize the grid with
    /// </summary>
    /// <remarks>
    /// Each instance of this component should have its own options object
    /// </remarks>
    //[Parameter]
    internal IgbGridLiteOptions Options { get; set; } = new();

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
    /// <remarks>
    /// The event is cancellable which prevents the operation from being applied.
    /// The expression can be modified prior to the operation running.
    /// </remarks>
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
    /// <remarks>
    /// The event is cancellable which prevents the operation from being applied.
    /// The expression can be modified prior to the operation running.
    /// </remarks>
    [Parameter]
    public EventCallback<IgbGridLiteFilteringEventArgs> Filtering { get; set; }

    /// <summary>
    /// Fires when a filter operation initiated through the UI has completed.
    /// Returns the filter state for the affected column.
    /// </summary>
    [Parameter]
    public EventCallback<IgbGridLiteFilteredEventArgs> Filtered { get; set; }

    /// <summary>
    /// Fires when <see cref="RenderAsync"/> completes
    /// </summary>
    [Parameter]
    public EventCallback Rendered { get; set; }

    private ElementReference grid;
    private IJSObjectReference? blazorIgbGridLite;
    private JSHandler<TItem>? jsHandler;
    private readonly string gridId = Guid.NewGuid().ToString("N");
    private bool isInitialized;
    private bool forceRender = true;

    /// <summary>
    /// The unique identifier for this grid instance
    /// </summary>
    public string GridId => gridId;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Options ??= new IgbGridLiteOptions();
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !isInitialized)
        {
            blazorIgbGridLite = await JSLoader.LoadAsync(JSRuntime, Options?.JavascriptPath);
            isInitialized = true;
            jsHandler = new JSHandler<TItem>(this);
        }

        if (isInitialized && forceRender)
        {
            await RenderGridAsync();
        }
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        GridLiteUpdateConfig? updateConfig = null;

        if (isInitialized)
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

        if (updateConfig != null)
        {
            var json = JsonSerializer.Serialize(updateConfig, GridLiteJsonContext.Default.GridLiteUpdateConfig);
            await InvokeVoidJsAsync("blazor_igc_grid_lite.updateGrid", gridId, json);
        }
    }

    /// <summary>
    /// The render() method is responsible for drawing the grid on the page.
    /// It is the primary method that has to be called after configuring the options.
    /// </summary>
    public virtual async Task RenderAsync()
    {
        await RenderGridAsync();
    }

    private async Task RenderGridAsync()
    {
        if (!isInitialized || jsHandler is null)
            return;

        await Task.Yield();
        forceRender = false;

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
            Events = new GridLiteEventFlags
            {
                HasSorting = Sorting.HasDelegate,
                HasSorted = Sorted.HasDelegate,
                HasFiltering = Filtering.HasDelegate,
                HasFiltered = Filtered.HasDelegate,
            },
        };

        var json = JsonSerializer.Serialize(config, GridLiteJsonContext.Default.GridLiteRenderConfig);

        await InvokeVoidJsAsync("blazor_igc_grid_lite.renderGrid", jsHandler.ObjectReference, grid, json);

        await Rendered.InvokeAsync();
    }

    private static GridLiteDataPayload CreateDataPayload(IEnumerable<TItem>? data)
    {
        // The web component spreads `data`, so it cannot take null; a null parameter means "no rows".
        return new GridLiteDataPayload(data ?? Array.Empty<TItem>(), AppValueSerializer.GetDataTypeInfo<TItem>());
    }

    /// <summary>
    /// Refreshes the grid by re-rendering it with the current data and configuration.
    /// </summary>
    public virtual async Task RefreshAsync()
    {
        forceRender = true;
        await RenderGridAsync();
    }

    /// <summary>
    /// Updates the data source for the grid.
    /// </summary>
    /// <param name="newData">The new data to display in the grid</param>
    public virtual async Task UpdateDataAsync(IEnumerable<TItem> newData)
    {
        ArgumentNullException.ThrowIfNull(newData);
        Data = newData;
        var json = JsonSerializer.Serialize(newData, AppValueSerializer.GetDataTypeInfo<TItem>());
        await InvokeVoidJsAsync("blazor_igc_grid_lite.updateData", gridId, json);
    }

    /// <summary>
    /// Performs a sort operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The sort expression(s) to apply</param>
    public virtual async Task SortAsync(IgbGridLiteSortingExpression expressions)
    {
        ArgumentNullException.ThrowIfNull(expressions);
        var json = JsonSerializer.Serialize(expressions, GridLiteJsonContext.Default.IgbGridLiteSortingExpression);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.sort", gridId, json);
    }

    /// <summary>
    /// Performs a sort operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The sort expression(s) to apply</param>
    public virtual async Task SortAsync(List<IgbGridLiteSortingExpression> expressions)
    {
        ArgumentNullException.ThrowIfNull(expressions);
        var json = JsonSerializer.Serialize(expressions, GridLiteJsonContext.Default.ListIgbGridLiteSortingExpression);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.sort", gridId, json);
    }

    /// <summary>
    /// Resets the current sort state of the grid.
    /// </summary>
    /// <param name="key">Optional column field. If provided, only clears sort for that column.
    /// If null, clears all sorting.</param>
    public virtual async Task ClearSortAsync(string? key = null)
    {
        await InvokeVoidJsAsync("blazor_igc_grid_lite.clearSort", gridId, key);
    }

    /// <summary>
    /// Performs a filter operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The filter expression(s) to apply</param>
    public virtual async Task FilterAsync(IgbGridLiteFilterExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var json = JsonSerializer.Serialize(expression, GridLiteJsonContext.Default.IgbGridLiteFilterExpression);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.filter", gridId, json);
    }

    /// <summary>
    /// Performs a filter operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The filter expression(s) to apply</param>
    public virtual async Task FilterAsync(List<IgbGridLiteFilterExpression> expressions)
    {
        ArgumentNullException.ThrowIfNull(expressions);
        var json = JsonSerializer.Serialize(expressions, GridLiteJsonContext.Default.ListIgbGridLiteFilterExpression);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.filter", gridId, json);
    }

    /// <summary>
    /// Resets the current filter state of the grid.
    /// </summary>
    /// <param name="key">Optional column field. If provided, only clears filter for that column.
    /// If null, clears all filtering.</param>
    public virtual async Task ClearFilterAsync(string? key = null)
    {
        await InvokeVoidJsAsync("blazor_igc_grid_lite.clearFilter", gridId, key);
    }

    /// <summary>
    /// Returns the current column configuration list.
    /// </summary>
    /// <returns>The column configurations</returns>
    public async ValueTask<IgbColumnConfiguration[]> GetColumnsAsync()
    {
        var columns = await InvokeJsAsync("blazor_igc_grid_lite.getColumns", gridId);
        return columns is { ValueKind: JsonValueKind.Array } array
            ? array.Deserialize(GridLiteJsonContext.Default.IgbColumnConfigurationArray) ?? []
            : [];
    }

    /// <summary>
    /// Navigates to a position in the grid based on provided row index and column field.
    /// </summary>
    /// <param name="row">The row index to navigate to</param>
    /// <param name="field">The column field to navigate to, if any</param>
    /// <param name="activate">Optionally also activate the navigated cell</param>
    /// <returns></returns>
    public virtual async Task NavigateToAsync(long row, string? field = null, bool activate = false)
    {
        await InvokeVoidJsAsync("blazor_igc_grid_lite.navigateTo", gridId, row, field, activate);
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
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);

        if (gridId != null && isInitialized && blazorIgbGridLite != null)
        {
            try
            {
                _ = InvokeAsync(async () =>
                {
                    await InvokeVoidJsAsync("blazor_igc_grid_lite.destroyGrid", gridId);
                });
                _ = InvokeAsync(async () =>
                {
                    await blazorIgbGridLite.DisposeAsync();
                });
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is JSDisconnectedException)
            { }
        }

        jsHandler?.Dispose();
    }
}
