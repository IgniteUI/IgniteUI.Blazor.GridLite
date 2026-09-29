using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// Source-generated serializer metadata for every payload the library owns. The app-owned values inside
/// them go through <see cref="AppValueSerializer"/>: <c>Data</c> through <see cref="GridLiteDataPayload"/>,
/// the object-typed filter values through <see cref="FilterValueConverter"/>.
/// </summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = [typeof(FilterValueConverter)])]
[JsonSerializable(typeof(GridLiteRenderConfig))]
[JsonSerializable(typeof(GridLiteUpdateConfig))]
[JsonSerializable(typeof(IgbGridLiteSortingExpression))]
[JsonSerializable(typeof(List<IgbGridLiteSortingExpression>))]
[JsonSerializable(typeof(IgbGridLiteFilterExpression))]
[JsonSerializable(typeof(List<IgbGridLiteFilterExpression>))]
[JsonSerializable(typeof(IgbColumnConfiguration[]))]
[JsonSerializable(typeof(IgbGridLiteFilteringEventArgs))]
[JsonSerializable(typeof(IgbGridLiteFilteredEventArgs))]
internal partial class GridLiteJsonContext : JsonSerializerContext
{
}
