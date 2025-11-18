using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Configuration for customizing the various data operations of the grid.
/// </summary>
internal class DataPipelineConfiguration
{
    /// <summary>
    /// Hook for customizing sort operations.
    /// </summary>
    [JsonIgnore]
    public Func<DataPipelineParams, Task<object[]>> Sort { get; set; }

    /// <summary>
    /// Hook for customizing filter operations.
    /// </summary>
    [JsonIgnore]
    public Func<DataPipelineParams, Task<object[]>> Filter { get; set; }
}

/// <summary>
/// The parameters passed to a DataPipelineHook callback.
/// </summary>
internal class DataPipelineParams
{
    /// <summary>
    /// The current data state of the grid.
    /// </summary>
    [JsonPropertyName("data")]
    public object[] Data { get; set; }

    /// <summary>
    /// The type of data operation being performed.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } // "sort" or "filter"
}