# PublishSmoke — trimmed-publish verification app

A minimal Blazor WebAssembly app that publishes `IgniteUI.Blazor.GridLite` **with assembly trimming** and exercises its reflection-based and interop paths in a real browser. The trim analyzer cannot validate `[UnconditionalSuppressMessage]` justifications, or whether ILLink honors an annotation the way the library relies on. This app is the end-to-end check for those.

## Why publish? Can't I just `dotnet run -c Release`?

No. The trimmer (ILLink) only runs during **publish**. `dotnet run` and `dotnet build` always use the full, untrimmed assemblies, so nothing trim-related can be observed that way. Verification is always: publish, then serve the publish output as static files.

## Why `TrimMode=full`

Blazor WebAssembly publishes with `TrimMode=partial` by default, which trims only assemblies marked trimmable. That includes the library, but never the app's own types. The library's promise about app types (the grid's item type keeps its public properties) only matters, and can only be checked, when the app's types are trimmed too. For the same reason `SmokeItem` and `SmokeDetail` are top-level types: types nested in a component class are kept along with it.

A control run shows the gate working. With the app's `DynamicDependency` for `SmokeDetail` removed, the nested `Code` column renders empty while the root item's columns still render.

## Run it

```bash
# 1. Publish trimmed (from the repo root). The app multi-targets the library's TFMs;
#    publish requires picking one, so check all three after linker-sensitive changes.
dotnet publish tests/IgniteUI.Blazor.GridLite.PublishSmoke -c Release -f net10.0

# 2. Serve the publish output (any static file server works; npx needs no install, since Node is a repo prerequisite):
npx http-server tests/IgniteUI.Blazor.GridLite.PublishSmoke/bin/Release/net10.0/publish/wwwroot -p 5620

# 3. Open http://localhost:5620/ and check the page against the table below.
```

Three gates, in order of what they can see:

1. **Build**: the trim analyzer runs over this app's own source (`EnableTrimAnalyzer`, findings are errors via the repo `.editorconfig`); the library's source is analyzed the same way in its own build. Build-time analysis cannot see linker behavior.
2. **Publish**: ILLink runs, and the publish fails on any linker warning (`ILLinkTreatWarningsAsErrors`, single-warn off). Linker _analysis_ warnings stay off, as is the Blazor default, because the framework's own assemblies emit them.
3. **Browser**: the only gate that catches _silent_ trims. A publish can succeed while an annotation or a suppression's justification has quietly stopped holding. That's the checklist below.

## What to check in the browser

| Page section                        | Expected                                         | A failure means                                                                                                             |
| ----------------------------------- | ------------------------------------------------ | --------------------------------------------------------------------------------------------------------------------------- |
| Grid rows                           | `Alpha`/`Beta`/`Gamma` with their prices         | the item type's properties were trimmed: the grid's `DynamicallyAccessedMembers` annotation on `TItem` no longer keeps them |
| `Code` column                       | `A-1`/`B-2`/`C-3`                                | the nested type's properties were trimmed: the documented `DynamicDependency` pattern no longer suffices                    |
| Click the `Name` header's sort icon | the line below the button reads `Name Ascending` | the `sorted` event payload no longer deserializes, or the `[JSInvokable]` handler was trimmed                               |
| Click `Price greater than 10`       | two rows remain                                  | the numeric search term no longer serializes through the library's reflection-based path                                    |
| Browser console                     | no errors (a stray `favicon` 404 is fine)        | anything else: investigate                                                                                                  |

The checklist is automated as `TrimmedPublishSmokeTest` in `IgniteUI.Blazor.GridLite.IntegrationTests` (`Category=TrimmedPublish`, runs in CI after the publish step). The automation also covers the `filtered` event payload, which the page only receives from grid-lite's filter row. It serves the net10.0 publish output, publishing it on demand if missing, so locally it's just:

```bash
dotnet test tests/IgniteUI.Blazor.GridLite.IntegrationTests --filter Category=TrimmedPublish --settings .runsettings
```

The manual browser pass above remains useful for the other TFMs and for linker experiments.

## When to run

- After any change to reflection, serialization, `[DynamicallyAccessedMembers]` annotations or suppressions in `src/` (see docs/TRIMMING.md).
- After SDK updates, since linker behavior should be re-validated.
- For linker experiments, run a control (publish _without_ the change) first, and delete `obj/Release/<tfm>/linked` between runs: ILLink's incremental check can skip a publish whose only change is a property such as `TrimMode`.
