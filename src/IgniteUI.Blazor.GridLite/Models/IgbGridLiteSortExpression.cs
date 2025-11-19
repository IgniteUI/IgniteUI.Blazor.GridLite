using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Represents a sort operation for a given column.
/// </summary>
public class IgbGridLiteSortExpression
{
    /// <summary>
    /// The target column.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// Sort direction for this operation.
    /// </summary>
    [JsonPropertyName("direction")]
    public GridLiteSortingDirection Direction { get; set; }

    /// <summary>
    /// Whether the sort operation should be case sensitive.
    /// If not provided, the value is resolved based on the column sort configuration.
    /// </summary>
    [JsonPropertyName("caseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CaseSensitive { get; set; }

    /// <summary>
    /// Custom comparer function for this operation.
    /// Note: This is not directly supported in Blazor and would need JavaScript interop.
    /// </summary>
    [JsonIgnore]
    internal Func<object, object, int> Comparer { get; set; }
}

/// <summary>
/// Sort direction for a given sort expression.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<GridLiteSortingDirection>))]
public enum GridLiteSortingDirection
{
    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties#custom-enum-member-names
    //[JsonStringEnumMemberName("ascending")] // .NET9+
    Ascending,

    //[JsonStringEnumMemberName("descending")] // .NET9+
    Descending,

    //[JsonStringEnumMemberName("none")] // .NET9+
    None
}