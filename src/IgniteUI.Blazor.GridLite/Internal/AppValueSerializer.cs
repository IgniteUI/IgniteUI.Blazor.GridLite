using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// Reflection-based metadata for the values whose types belong to the app: the <c>Data</c> items and the
/// object-typed filter values, which are compared against them. Everything else goes through
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
            + "item type's public properties in trimmed apps, and filter values of built-in types need no "
            + "preserved members. Any other type reached here (a complex type nested in TItem, an app-defined "
            + "filter value) belongs to the consuming app, and the trimming docs make preserving it the app's "
            + "responsibility.")]
    private static JsonSerializerOptions CreateOptions()
    {
        // No naming policy: data keys keep the C# property names that IgbGridLiteColumn.Field matches via nameof.
        return new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        };
    }
}

/// <summary>
/// Writes an <see cref="object"/>-typed payload member (the filter <c>Condition</c> and <c>SearchTerm</c>) by
/// its runtime type, the way the <c>Data</c> items are written, and reads it back as a
/// <see cref="JsonElement"/>.
/// </summary>
internal sealed class FilterValueConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonElement.ParseValue(ref reader);

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, AppValueSerializer.GetValueTypeInfo(value));
}
