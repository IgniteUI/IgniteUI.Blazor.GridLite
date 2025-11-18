using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Options for the Blazor IgbGridLite component.
/// </summary>
internal class IgbGridLiteOptions
{
    /// <summary>
    /// Alternative file path to the Blazor JavaScript file, blazor-igc-grid-lite.js.
    /// This must be the full path including filename.
    /// </summary>
    [JsonPropertyName("javascriptPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string JavascriptPath { get; set; }

    /// <summary>
    /// Configuration object which controls remote data operations for the grid.
    /// </summary>
    [JsonPropertyName("dataPipelineConfiguration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DataPipelineConfiguration DataPipelineConfiguration { get; set; }

    /// <summary>
    /// Specifies whether to enable debug mode.
    /// </summary>
    [JsonPropertyName("debug")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Debug { get; set; }
}