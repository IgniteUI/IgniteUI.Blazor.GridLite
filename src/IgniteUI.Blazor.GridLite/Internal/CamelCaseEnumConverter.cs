using System.Text.Json;
using System.Text.Json.Serialization;

namespace IgniteUI.Blazor.Controls.Internal;

/// <summary>
/// Per https://github.com/dotnet/runtime/issues/35163 (kinda, CreateConverter is sealed now..)
/// </summary>
internal class CamelCaseEnumConverter<T> : JsonStringEnumConverter<T>
    where T : struct, Enum
{
    public CamelCaseEnumConverter()
        : base(JsonNamingPolicy.CamelCase, allowIntegerValues: false) { }
}
