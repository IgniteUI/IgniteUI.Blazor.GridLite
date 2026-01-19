# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

This release updates to `igniteui-grid-lite` version `0.3.1` with a new declarative column API.

### Added

- New `IgbGridLiteColumn` component for declarative column definition
- Updated to `igniteui-grid-lite` version `~0.3.1`

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

[Unreleased]: https://github.com/IgniteUI/IgniteUI.Blazor.GridLite/compare/v0.3.1...HEAD
[0.3.1]: https://github.com/IgniteUI/IgniteUI.Blazor.GridLite/releases/tag/v0.3.1
