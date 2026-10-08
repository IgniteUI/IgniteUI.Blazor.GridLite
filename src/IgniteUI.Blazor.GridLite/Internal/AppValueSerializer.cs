using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// Reflection-based metadata for the values whose types belong to the app: the <c>Data</c> items and the filter
/// expressions' <c>SearchTerm</c>, which is compared against them. Everything else goes through
/// <see cref="GridLiteJsonContext"/>.
/// </summary>
internal static class AppValueSerializer
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    public static JsonTypeInfo<IEnumerable<TItem>> GetDataTypeInfo<TItem>()
        => (JsonTypeInfo<IEnumerable<TItem>>)Options.GetTypeInfo(typeof(IEnumerable<TItem>));

    public static JsonTypeInfo GetValueTypeInfo(object value) => Options.GetTypeInfo(value.GetType());

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "The grid's DynamicallyAccessedMembers(PublicProperties) annotation on TItem keeps the "
            + "item type's public properties in trimmed apps, and filter search terms of built-in types need no "
            + "preserved members. Any other type reached here (a complex type nested in TItem, an app-defined "
            + "filter search term) belongs to the consuming app, and the trimming docs make preserving it the app's "
            + "responsibility.")]
    private static JsonSerializerOptions CreateOptions()
    {
        // No naming policy: data keys keep the C# property names that IgbGridLiteColumn.Field matches via nameof.
        return new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // Native AOT is not claimed for this reflection-based serialization. A pragma, unlike
            // UnconditionalSuppressMessage, silences only the library build: a PublishAot app still gets the warning.
#pragma warning disable IL3050
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
#pragma warning restore IL3050
        };
    }
}

/// <summary>
/// Writes the filter expressions' <c>SearchTerm</c>, the only <see cref="object"/>-typed payload member, by
/// its runtime type, the way the <c>Data</c> items are written, and reads it back as the primitive it is.
/// </summary>
internal sealed class FilterValueConverter : JsonConverter<object>
{
    // JSON has one number type OOB, so every number reads as a double. An array or object can only be a value
    // the app set itself; it stays a JsonElement.
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString()!,
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            _ => JsonElement.ParseValue(ref reader),
        };

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, AppValueSerializer.GetValueTypeInfo(value));
}
