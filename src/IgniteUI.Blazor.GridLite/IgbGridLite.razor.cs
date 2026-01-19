using IgniteUI.Blazor.Controls.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// IgbGridLite is a component for displaying data in a tabular format quick and easy.
/// </summary>
/// <typeparam name="TItem">The data type of the items to display in the grid</typeparam>
public partial class IgbGridLite<TItem> : ComponentBase, IDisposable where TItem : class
{
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// The data to display in the grid
    /// </summary>
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
    private IJSObjectReference blazorIgbGridLite;
    private JSHandler<TItem> jsHandler;
    private string gridId;
    private bool isInitialized;
    private bool forceRender = true;

    // Caching the JsonSerializerOptions instance as a static readonly field improves performance
    // by avoiding repeated allocations and configuration. Reusing serializer options is recommended
    // for high-frequency serialization operations, such as grid data updates.
    private static readonly JsonSerializerOptions GridJsonSerializerOptions = new()
    {
        // TODO: This policy might need to be configurable in the future (for data serialization at least)
        //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// The unique identifier for this grid instance
    /// </summary>
    public string GridId => gridId;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        gridId = Guid.NewGuid().ToString("N");
        Options ??= new IgbGridLiteOptions();
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"IgbGridLite<{typeof(TItem).Name}> OnAfterRenderAsync firstRender={firstRender} isInitialized={isInitialized} forceRender={forceRender}");

            if (firstRender && !isInitialized)
            {
                blazorIgbGridLite = await JSLoader.LoadAsync(JSRuntime, Options?.JavascriptPath);
                isInitialized = true;
                jsHandler = new JSHandler<TItem>(this);
                System.Diagnostics.Debug.WriteLine("IgbGridLite: JS initialized.");
            }

            if (isInitialized && forceRender)
            {
                await RenderGridAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"IgbGridLite OnAfterRenderAsync error: {ex}");
            throw;
        }
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var updateConfig = new Dictionary<string, object>();

        if (isInitialized)
        {
            if (parameters.TryGetValue<IEnumerable<TItem>?>(nameof(Data), out var newData) 
                && !ReferenceEquals(Data, newData))
            {
                updateConfig["data"] = newData;
            }
            
            if (parameters.TryGetValue<bool>(nameof(AutoGenerate), out var newAutoGenerate)
                && AutoGenerate != newAutoGenerate)
            {
                updateConfig["autoGenerate"] = newAutoGenerate;
            }
            
            if (parameters.TryGetValue<IgbGridLiteSortingOptions?>(nameof(SortingOptions), out var newSortOptions)
                && !ReferenceEquals(SortingOptions, newSortOptions))
            {
                updateConfig["sortingOptions"] = newSortOptions;
            }
            
            if (parameters.TryGetValue<IEnumerable<IgbGridLiteSortingExpression>?>(nameof(SortingExpressions), out var newSortingExpressions)
                && !ReferenceEquals(SortingExpressions, newSortingExpressions))
            {
                updateConfig["sortingExpressions"] = newSortingExpressions;
            }
            
            if (parameters.TryGetValue<IEnumerable<IgbGridLiteFilterExpression>?>(nameof(FilterExpressions), out var newFilterExpressions)
                && !ReferenceEquals(FilterExpressions, newFilterExpressions))
            {
                updateConfig["filterExpressions"] = newFilterExpressions;
            }
        }

        await base.SetParametersAsync(parameters);

        if (updateConfig.Count > 0)
        {
            var json = JsonSerializer.Serialize(updateConfig, GridJsonSerializerOptions);
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
        if (!isInitialized)
            return;

        await Task.Yield();
        forceRender = false;

        var config = new
        {
            id = gridId,
            data = Data,
            autoGenerate = AutoGenerate,
            sortingOptions = SortingOptions,
            sortingExpressions = SortingExpressions,
            filterExpressions = FilterExpressions
        };

        var json = JsonSerializer.Serialize(config, GridJsonSerializerOptions);

        await InvokeVoidJsAsync("blazor_igc_grid_lite.renderGrid",
            jsHandler.ObjectReference, grid, json, GetEventFlags());

        await Rendered.InvokeAsync();
    }

    private object GetEventFlags()
    {
        return new
        {
            hasSorting = Sorting.HasDelegate,
            hasSorted = Sorted.HasDelegate,
            hasFiltering = Filtering.HasDelegate,
            hasFiltered = Filtered.HasDelegate
        };
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
        Data = newData;
        var json = JsonSerializer.Serialize(newData, GridJsonSerializerOptions);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.updateData", gridId, json);
    }

    /// <summary>
    /// Performs a sort operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The sort expression(s) to apply</param>
    public virtual async Task SortAsync(IgbGridLiteSortingExpression expressions)
    {
        var json = JsonSerializer.Serialize(expressions, GridJsonSerializerOptions);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.sort", gridId, json);
    }

    /// <summary>
    /// Performs a sort operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The sort expression(s) to apply</param>
    public virtual async Task SortAsync(List<IgbGridLiteSortingExpression> expressions)
    {
        var json = JsonSerializer.Serialize(expressions, GridJsonSerializerOptions);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.sort", gridId, json);
    }

    /// <summary>
    /// Resets the current sort state of the grid.
    /// </summary>
    /// <param name="key">Optional column key. If provided, only clears sort for that column. 
    /// If null, clears all sorting.</param>
    public virtual async Task ClearSortAsync(string key = null)
    {
        await InvokeVoidJsAsync("blazor_igc_grid_lite.clearSort", gridId, key);
    }

    /// <summary>
    /// Performs a filter operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The filter expression(s) to apply</param>
    public virtual async Task FilterAsync(IgbGridLiteFilterExpression expression)
    {
        var json = JsonSerializer.Serialize(expression, GridJsonSerializerOptions);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.filter", gridId, json);
    }

    /// <summary>
    /// Performs a filter operation in the grid based on the passed expression(s).
    /// </summary>
    /// <param name="expressions">The filter expression(s) to apply</param>
    public virtual async Task FilterAsync(List<IgbGridLiteFilterExpression> expressions)
    {
        var json = JsonSerializer.Serialize(expressions, GridJsonSerializerOptions);
        await InvokeVoidJsAsync("blazor_igc_grid_lite.filter", gridId, json);
    }

    /// <summary>
    /// Resets the current filter state of the grid.
    /// </summary>
    /// <param name="key">Optional column key. If provided, only clears filter for that column. 
    /// If null, clears all filtering.</param>
    public virtual async Task ClearFilterAsync(string key = null)
    {
        await InvokeVoidJsAsync("blazor_igc_grid_lite.clearFilter", gridId, key);
    }

    /// <summary>
    /// Returns the current column configuration list.
    /// </summary>
    /// <param name="index">The zero-based column index</param>
    /// <returns>The column configuration if found, otherwise null</returns>
    public ValueTask<IgbColumnConfiguration[]> GetColumnsAsync()
    {
        return this.InvokeJsAsync<IgbColumnConfiguration[]>("getColumns");
    }

    private async ValueTask<TValue> InvokeJsAsync<TValue>(string identifier, params object[] args)
    {
        if (blazorIgbGridLite == null) { return default; }

        try
        {
            return await blazorIgbGridLite.InvokeAsync<TValue>(identifier, args);
        }
        catch (Exception ex) when (ex is ObjectDisposedException || ex is JSDisconnectedException)
        {
            return default;
        }
    }

    private async ValueTask InvokeVoidJsAsync(string identifier, params object[] args)
    {
        if (blazorIgbGridLite == null) return;

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