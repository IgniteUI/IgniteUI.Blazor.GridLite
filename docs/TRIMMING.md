# Trimming support

`IgniteUI.Blazor.GridLite` is trim-compatible (`IsTrimmable=true`): the library builds warning-free under the .NET trim analyzer, and its own interop payloads serialize through source-generated `System.Text.Json` metadata. The only values it serializes with reflection are your app's own: the grid's `Data` items and the filter expressions' `Condition`/`SearchTerm`.

## Are your types trimmed at all?

Blazor WebAssembly publishes with `TrimMode=partial` by default, which trims only assemblies marked trimmable. This library is trimmed, but your app's types are left intact, so the guidance below does not apply. It applies when your types are trimmed too: with `<TrimMode>full</TrimMode>`, or when the assembly that declares them is marked `IsTrimmable`.

## The grid's item type

The `Data` items are serialized with reflection, keeping their C# property names, which is what lets `IgbGridLiteColumn.Field` match them via `nameof`. `IgbGridLite<TItem>` annotates `TItem` with `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]`, so a grid used with a concrete item type keeps that type's public properties automatically:

```razor
<IgbGridLite TItem="Product" Data="products">...</IgbGridLite>
```

A component of your own that passes its generic parameter through as `TItem` has to carry the same annotation. Your app's trim analyzer reports IL2091 when it is missing:

```csharp
public partial class ProductGrid<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    : ComponentBase where T : class
{
    // renders <IgbGridLite TItem="T" ...>
}
```

### What is not kept automatically

- **Complex types reachable from the item type.** If `Product` has a `Supplier` property whose members a column shows (`Field="Supplier.Name"`), `Supplier` needs its public properties preserved as well, and so does every complex type below it. The annotation on `TItem` covers the item type itself, not the types of its properties.
- **Members outside the public properties.** A public field opted into serialization with `[JsonInclude]`, or a non-public `[JsonInclude]` member, is not covered by the annotation.
- **App-defined filter values.** A `Condition` or `SearchTerm` of a built-in type (string, number, date, `Guid`, enum) needs nothing. A value of an app-defined type needs its public properties preserved.

Preserve such types with a `DynamicDependency` attribute on any method that is kept, such as your root component or `Program.Main`:

```csharp
[DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(Supplier))]
```

or by annotating the type itself:

```csharp
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class Supplier { ... }
```

or with a [trimmer root descriptor](https://learn.microsoft.com/dotnet/core/deploying/trimming/trimming-options#root-descriptors) that lists the types. Prefer the `DynamicDependency` form: it holds regardless of how the type is reached, while the attribute on the type only takes effect where the trimmer can see the type flowing into an annotated location.

## Native AOT

Not claimed. The `Data` items and the filter values are serialized with reflection, which needs runtime type information that Native AOT does not guarantee. Blazor WebAssembly, including publishes with `RunAOTCompilation=true`, runs on Mono with the interpreter available and is unaffected.

## Maintaining trim compatibility (contributors)

The library must stay trim-clean. Every trim-analysis diagnostic builds as an error (`dotnet_analyzer_diagnostic.category-Trimming.severity = error` in the repo `.editorconfig`; the category bulk-config covers future IL2xxx codes too, and is inert where the analyzer is off). The analyzer cannot validate suppression justifications, or whether ILLink honors an annotation the way the library relies on. `tests/IgniteUI.Blazor.GridLite.PublishSmoke` is the authoritative check for those, automated as the `TrimmedPublish`-category browser tests in `IgniteUI.Blazor.GridLite.IntegrationTests` (net10.0, runs in CI); its README has the manual checklist covering the other TFMs.

When the analyzer flags new code, follow the standard playbook: avoid reflection and dynamic code where possible, otherwise annotate or fix the root cause, and suppress only as a last resort. See [preparing libraries for trimming: recommendations](https://learn.microsoft.com/dotnet/core/deploying/trimming/prepare-libraries-for-trimming#recommendations) and [resolving trim warnings](https://learn.microsoft.com/dotnet/core/deploying/trimming/fixing-warnings). Repo-specific rules on top:

1. **Serialization goes through source-generated `JsonTypeInfo`.** Extend `GridLiteJsonContext` rather than calling reflection-based `JsonSerializer` overloads. App-owned values go through `AppValueSerializer`, the library's only reflection-based serialization.
2. **Never annotate _method parameters or fields_ of component classes** with `[DynamicallyAccessedMembers]`. `OpenComponent<T>` roots component members "via reflection", and the annotation then surfaces as IL2111/IL2110 in every consuming app. Annotated properties and type parameters are fine.
3. **Suppress narrowly.** Put `[UnconditionalSuppressMessage]` on the smallest member, with a justification that states why the pattern is safe; extract a small helper if needed, so that the justification matches exactly what the member does.
4. **Never use `#pragma` for ILxxxx.** It silences only the build analyzer and leaves no metadata for publish-time trim tooling (ILLink, NativeAOT's ILCompiler).
