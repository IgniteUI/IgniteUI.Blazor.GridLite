using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// The grid properties, as sent by <c>updateGrid</c>: null members are left out, so the payload carries
/// only the parameters that changed.
/// </summary>
internal class GridLiteUpdateConfig
{
    [JsonPropertyName("data")]
    public GridLiteDataPayload? Data { get; set; }

    [JsonPropertyName("autoGenerate")]
    public bool? AutoGenerate { get; set; }

    [JsonPropertyName("adoptRootStyles")]
    public bool? AdoptRootStyles { get; set; }

    [JsonPropertyName("sortingOptions")]
    public IgbGridLiteSortingOptions? SortingOptions { get; set; }

    [JsonPropertyName("sortingExpressions")]
    public IEnumerable<IgbGridLiteSortingExpression>? SortingExpressions { get; set; }

    [JsonPropertyName("filterExpressions")]
    public IEnumerable<IgbGridLiteFilterExpression>? FilterExpressions { get; set; }
}

/// <summary>
/// The <c>renderGrid</c> payload: every grid property, plus the grid id and the bound events.
/// </summary>
internal sealed class GridLiteRenderConfig : GridLiteUpdateConfig
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("events")]
    public required GridLiteEventFlags Events { get; init; }
}

/// <summary>
/// Which grid events have a bound callback; <c>renderGrid</c> attaches listeners only for those.
/// </summary>
internal sealed class GridLiteEventFlags
{
    [JsonPropertyName("hasSorting")]
    public bool HasSorting { get; init; }

    [JsonPropertyName("hasSorted")]
    public bool HasSorted { get; init; }

    [JsonPropertyName("hasFiltering")]
    public bool HasFiltering { get; init; }

    [JsonPropertyName("hasFiltered")]
    public bool HasFiltered { get; init; }
}

/// <summary>
/// The user data embedded in a library payload, written through the type info the grid resolved for
/// its item type.
/// </summary>
[JsonConverter(typeof(GridLiteDataPayloadConverter))]
internal sealed class GridLiteDataPayload(object items, JsonTypeInfo typeInfo)
{
    public object Items { get; } = items;

    public JsonTypeInfo TypeInfo { get; } = typeInfo;
}

internal sealed class GridLiteDataPayloadConverter : JsonConverter<GridLiteDataPayload>
{
    public override GridLiteDataPayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, GridLiteDataPayload value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value.Items, value.TypeInfo);
}
