# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

### Added

- The package includes `THIRD-PARTY-LICENSES.md` with the license texts of the JavaScript dependencies bundled into the grid's script.
- The library is trim-compatible (`IsTrimmable`). Only the app's own values are serialized with reflection: the `Data` items and the filter expressions' `Condition`/`SearchTerm`. Blazor WebAssembly's default `TrimMode=partial` leaves those types untrimmed. With `TrimMode=full`, a grid with a concrete item type keeps that type's public properties automatically; a component that passes its own generic parameter as `TItem` must annotate it with `[DynamicallyAccessedMembers(PublicProperties)]`, and complex types nested in the item type must be preserved by the app.
- The library's own interop code is AOT-safe: it needs no runtime code generation, and its build rejects any that is added. Native AOT is not claimed, since the app's values above are still serialized with reflection.

### Changed

- The package no longer ships a source map for the grid's script (`blazor-igc-grid-lite.js.map`).
- **BREAKING**: `IgbGridLiteSortingExpression.Key`, `IgbGridLiteFilterExpression.Key`, `IgbGridLiteFilterExpression.Condition` and `IgbColumnConfiguration.Field` are now `required`; object initializers must set them. `IgbGridLiteColumn.Field` is marked `[EditorRequired]`.
- The event-args members the web component always sends are now `required` (only code that constructs event args itself, such as tests, is affected): `IgbGridLiteSortingEventArgs.Expression`, `IgbGridLiteSortedEventArgs.Expression`, `IgbGridLiteFilteringEventArgs.Key`/`Expressions`/`Type` and `IgbGridLiteFilteredEventArgs.Key`/`State`.
- The same event-args members are `init`-only: a handler's changes to them never reached the grid.
- `GetColumnsAsync` returns an empty array instead of `null` when there is no grid on the client. Nullable annotations on the public surface: the optional `key` of `ClearSortAsync`/`ClearFilterAsync` and `field` of `NavigateToAsync` are `string?`. `IgbGridLiteFilterExpression.SearchTerm` and `Criteria` are nullable.
- `UpdateDataAsync`, `SortAsync` and `FilterAsync` throw `ArgumentNullException` for a null argument instead of sending `null` to the grid.
- **BREAKING**: The public methods of `IgbGridLite<TItem>` are no longer `virtual`.
- **BREAKING**: The multi-expression overloads of `SortAsync` and `FilterAsync` take `IEnumerable<T>` instead of `List<T>`. Source-compatible; recompile against the new version.
- `Rendered` fires once, when the grid first renders on the client; it no longer fires again after `RenderAsync` or `RefreshAsync`.
- The grid's methods, such as `SortAsync`, `FilterAsync`, `NavigateToAsync` and `GetColumnsAsync`, wait for the grid's first client render when called before it, instead of doing nothing.
- `IgbGridLite<TItem>` implements `IAsyncDisposable` instead of `IDisposable`, and its disposal completes once the client-side grid is released.
- The grid's script keeps its state in the module instead of on `window`: the undocumented `window.IgcGridLite` and `window.blazor_igc_grid_lite` globals are gone.

### Deprecated

- `GridLiteColumnDataType.Date`: the grid-lite web component has no date data type, so this value behaves like `String`. It will be removed in a future release.
- `RenderAsync` and `RefreshAsync`: the grid renders on its own and updates from its parameters. To show changed data or expressions, assign a new collection instead of changing one in place.
- `UpdateDataAsync`: assign a new collection to `Data` instead.
- `GridId`: not needed to use the grid.

### Fixed

- The package includes the XML documentation, so IntelliSense shows the descriptions of the grid's members ([#15](https://github.com/IgniteUI/IgniteUI.Blazor.GridLite/issues/15)).
- A null `Data` no longer causes a client error. Before, a grid rendered without data threw initially, and resetting `Data` to null left the previous rows on screen. Null is now sent as an empty array in both cases.
- Resetting `SortingOptions`, `SortingExpressions` or `FilterExpressions` to null restores the grid's default (multiple sorting, no sort, no filter) on the client.
- The `Sorting` and `Filtering` docs no longer claim the events can cancel or modify the operation; they are notifications.
- `RenderAsync` and `RefreshAsync` no longer add another set of event listeners, which raised every event callback once more per call.
- A `Sorting`, `Sorted`, `Filtering` or `Filtered` callback bound after the grid's first render fires; before, the grid did not listen for it.
- An exception from a `Sorting`, `Sorted`, `Filtering` or `Filtered` handler, or from reading the event's payload, is no longer swallowed; the browser reports it as an unhandled error.
- A grid removed while its script is still loading no longer renders on the client afterwards.

## 0.9.0 - 2026-07-13

This release updates to `igniteui-grid-lite` version `0.9.0` ([see changelog](https://github.com/IgniteUI/igniteui-grid-lite/blob/master/CHANGELOG.md)) with the following changes:

### Added

- Filtering row input enhancements with updated styles and refactored common logic.

### Changed

- Improved sort and filter performance for large data sets by precomputing sort keys and filter expression trees.
- Updated body cell and row theming to use the latest `igniteui-theming` variables.
- Updated `igniteui-webcomponents` to 7.2.0 and other dependencies.

### Fixed

- Body cell and row border styles now use the correct theme variables across default, odd, even, hover, and active states.

## 0.7.1 - 2026-04-29

This release updates to `igniteui-grid-lite` version `0.7.1` ([see changelog](https://github.com/IgniteUI/igniteui-grid-lite/blob/master/CHANGELOG.md)) with the following bug fixes:

### Fixed

- `filterExpressions` and `sortingExpressions` property setters now replace the existing state instead of being additive.
- Setting initial filter/sort state without column configuration no longer crashes.
- Adopted styles are now correctly applied on connected callback.

## 0.6.0 - 2026-03-04

This release updates to `igniteui-grid-lite` version `0.6.0` ([see changelog](https://github.com/IgniteUI/igniteui-grid-lite/blob/master/CHANGELOG.md)) with significant wrapper changes listed below.

### Added

- New `AdoptRootStyles` parameter for property for adopting document-level styles into shadow DOM when using cell and header templates.
- Updated theming and component size handling across grid styles - now supports sizing via the `--ig-size` CSS variable.

## 0.4.0 - 2026-02-02

This release updates to `igniteui-grid-lite` version `0.4.0` with a new declarative column API.

### Added

- New `IgbGridLiteColumn` component for declarative column definition
- New `NavigateToAsync` method. Navigates to a position in the grid based on provided row index and column field.
- Updated to `igniteui-grid-lite` version `~0.4.0` with multiple bug fixes

### Changed

- **BREAKING**: Column configuration is now declarative using `<IgbGridLiteColumn>` child elements instead of the `Columns` parameter
  ```razor
  <!-- Before -->
  <IgbGridLite Data="@data" Columns="@columns" />
  @code {
    private List<IgbColumnConfiguration> columns = new()
    {
      new() { Key = "Id", HeaderText = "ID", Type = GridLiteColumnDataType.Number }
    };
  }

  <!-- After -->
  <IgbGridLite Data="@data">
      <IgbGridLiteColumn Field="Id" Header="ID" DataType="GridLiteColumnDataType.Number" />
  </IgbGridLite>
  ```
- **BREAKING**: Column property renames:
  - `Key`→`Field`
  - `Type`→`DataType`
  - `HeaderText`→`Header`
- **BREAKING**: Column sort/filter configuration simplified:
  - `Sort` object → `Sortable` (bool) and `SortingCaseSensitive` (bool)
  - `Filter` object → `Filterable` (bool) and `FilteringCaseSensitive` (bool)
- **BREAKING**: Renamed `IgbGridLiteSortConfiguration` → `IgbGridLiteSortingOptions`
- **BREAKING**: Renamed `IgbGridLiteSortExpression` → `IgbGridLiteSortingExpression`
- **BREAKING**: Grid parameter renames:
  - `SortConfiguration`→`SortingOptions`
  - `SortExpressions`→`SortingExpressions`
- **BREAKING**: `IgbGridLiteSortingOptions.Multiple` (bool) → `Mode` (enum: `GridLiteSortingMode.Single` or `GridLiteSortingMode.Multiple`)

### Removed

- **BREAKING**: `Columns` parameter and `UpdateColumnsAsync()` method - use declarative `<IgbGridLiteColumn>` elements with conditional rendering instead
- **BREAKING**: `IgbColumnSortConfiguration` and `IgbColumnFilterConfiguration` classes
- **BREAKING**: `IgbGridLiteSortingOptions.TriState` property. Tri-state sorting is always enabled
