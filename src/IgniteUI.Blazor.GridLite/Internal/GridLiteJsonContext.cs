using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// Source-generated serializer metadata for every payload the library owns. The app-owned values inside
/// them go through <see cref="AppValueSerializer"/>: <c>Data</c> through <see cref="GridLiteDataPayload"/>,
/// the filter expressions' <c>SearchTerm</c> through <see cref="FilterValueConverter"/>.
/// </summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = [typeof(FilterValueConverter)])]
[JsonSerializable(typeof(GridLiteRenderConfig))]
[JsonSerializable(typeof(GridLiteUpdateConfig))]
[JsonSerializable(typeof(IgbGridLiteSortingExpression))]
[JsonSerializable(typeof(IEnumerable<IgbGridLiteSortingExpression>))]
[JsonSerializable(typeof(IgbGridLiteFilterExpression))]
[JsonSerializable(typeof(IEnumerable<IgbGridLiteFilterExpression>))]
[JsonSerializable(typeof(IgbColumnConfiguration[]))]
[JsonSerializable(typeof(IgbGridLiteFilteringEventArgs))]
[JsonSerializable(typeof(IgbGridLiteFilteredEventArgs))]
internal partial class GridLiteJsonContext : JsonSerializerContext
{
}
