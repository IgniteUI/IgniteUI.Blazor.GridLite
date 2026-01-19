# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Breaking Changes - Migration from igniteui-grid-lite 0.0.1 to 0.3.1

This release updates the wrapper to use `igniteui-grid-lite` version `0.3.1`, which includes several breaking changes to align with the new declarative column API.

#### 1. Declarative Columns (CRITICAL CHANGE)

**Old API (0.0.1):**
```razor
<IgbGridLite Data="@data" Columns="@columns" />

@code {
    private List<IgbColumnConfiguration> columns = new()
    {
        new() { Key = "Id", HeaderText = "ID", Type = GridLiteColumnDataType.Number }
    };
}
```

**New API (0.3.1):**
```razor
<IgbGridLite Data="@data">
    <IgbGridLiteColumn Field="Id" Header="ID" DataType="GridLiteColumnDataType.Number" />
</IgbGridLite>
```

- Column configuration is now **declarative** using `<IgbGridLiteColumn>` child elements
- The `Columns` parameter and `UpdateColumnsAsync()` method have been **removed** - columns can be updated declaratively using conditional rendering

#### 2. Column Property Renames

| Old Property | New Property | Notes |
|--------------|--------------|-------|
| `Key` | `Field` | The field from the data that the column references |
| `Type` | `DataType` | The data type of the column's values |
| `HeaderText` | `Header` | The header text of the column |

#### 3. Sort/Filter Property Changes

**Column Sort Configuration:**
- Old: `Sort` property (object or bool)
- New: `Sortable` (bool) and `SortingCaseSensitive` (bool?)

**Before:**
```csharp
new IgbColumnConfiguration {
    Sort = new IgbColumnSortConfiguration { CaseSensitive = false }
}
```

**After:**
```razor
<IgbGridLiteColumn Sortable SortingCaseSensitive="@false" />
```

**Column Filter Configuration:**
- Old: `Filter` property (object or bool)
- New: `Filterable` (bool) and `FilteringCaseSensitive` (bool?)

**Before:**
```csharp
new IgbColumnConfiguration {
    Filter = new IgbColumnFilterConfiguration { CaseSensitive = false }
}
```

**After:**
```razor
<IgbGridLiteColumn Filterable FilteringCaseSensitive="@false" />
```

#### 4. Sorting API Renames

| Old Type/Property | New Type/Property | Notes |
|-------------------|-------------------|-------|
| `IgbGridLiteSortConfiguration` | `IgbGridLiteSortingOptions` | Class renamed |
| `IgbGridLiteSortExpression` | `IgbGridLiteSortingExpression` | Class renamed |
| `SortConfiguration` parameter | `SortingOptions` parameter | Grid parameter renamed |
| `SortExpressions` parameter | `SortingExpressions` parameter | Grid parameter renamed |
| `Multiple` property (bool) | `Mode` property | Use `GridLiteSortingMode.Single` or `GridLiteSortingMode.Multiple` |
| `TriState` property | *Removed* | Tri-state sorting is always enabled |

**Before:**
```csharp
var sortConfig = new IgbGridLiteSortConfiguration {
    Multiple = true,
    TriState = false
};

var expression = new IgbGridLiteSortExpression {
    Key = "Name",
    Direction = GridLiteSortingDirection.Ascending
};
```

**After:**
```csharp
var sortingOptions = new IgbGridLiteSortingOptions {
    Mode = GridLiteSortingMode.Multiple
};

var expression = new IgbGridLiteSortingExpression {
    Key = "Name",
    Direction = GridLiteSortingDirection.Ascending
};
```

#### 5. Removed Types

The following types have been removed as they are no longer needed:
- `IgbColumnSortConfiguration` - replaced by boolean properties on column
- `IgbColumnFilterConfiguration` - replaced by boolean properties on column

#### 6. Removed Methods

- `UpdateColumnsAsync()` - columns are now declarative and can be updated using conditional rendering

### Migration Guide

1. **Replace column configuration with declarative syntax:**
   - Remove the `Columns` parameter from `<IgbGridLite>`
   - Add `<IgbGridLiteColumn>` child elements inside `<IgbGridLite>`
   - Update property names: `Key`→`Field`, `Type`→`DataType`, `HeaderText`→`Header`

2. **Update sort/filter configuration:**
   - Replace `Sort = true` with `Sortable` attribute
   - Replace `Filter = true` with `Filterable` attribute
   - Replace `Sort = new IgbColumnSortConfiguration { CaseSensitive = false }` with `Sortable SortingCaseSensitive="@false"`
   - Replace `Filter = new IgbColumnFilterConfiguration { CaseSensitive = false }` with `Filterable FilteringCaseSensitive="@false"`

3. **Update sorting configuration:**
   - Rename `IgbGridLiteSortConfiguration` to `IgbGridLiteSortingOptions`
   - Rename `IgbGridLiteSortExpression` to `IgbGridLiteSortingExpression`
   - Change `SortConfiguration` parameter to `SortingOptions`
   - Change `SortExpressions` parameter to `SortingExpressions`
   - Replace `Multiple = true` with `Mode = GridLiteSortingMode.Multiple`
   - Remove `TriState` property usage

4. **Dynamic columns:**
   - Instead of calling `UpdateColumnsAsync()`, use conditional rendering with `@if` statements around `<IgbGridLiteColumn>` elements

### Added

- New `IgbGridLiteColumn` component for declarative column definition
- `ChildContent` parameter on `IgbGridLite` for declarative columns
- Updated to `igniteui-grid-lite` version `~0.3.1`

### Changed

- `IgbColumnConfiguration`: Renamed `Key`→`Field`, `Type`→`DataType`, `HeaderText`→`Header`
- `IgbColumnConfiguration`: Replaced `Sort` object with `Sortable` (bool) and `SortingCaseSensitive` (bool?)
- `IgbColumnConfiguration`: Replaced `Filter` object with `Filterable` (bool) and `FilteringCaseSensitive` (bool?)
- Renamed `IgbGridLiteSortConfiguration` to `IgbGridLiteSortingOptions`
- Renamed `IgbGridLiteSortExpression` to `IgbGridLiteSortingExpression`
- `IgbGridLiteSortingOptions`: Replaced `Multiple` (bool) with `Mode` (enum `GridLiteSortingMode.Single` or `GridLiteSortingMode.Multiple`)
- Grid parameters: `SortConfiguration`→`SortingOptions`, `SortExpressions`→`SortingExpressions`
- Event args: Updated to use new `IgbGridLiteSortingExpression` type

### Removed

- `IgbGridLiteSortingOptions.TriState` property
- `IgbColumnSortConfiguration` class
- `IgbColumnFilterConfiguration` class
- `Columns` parameter and `UpdateColumnsAsync()` method from `IgbGridLite` - use declarative `<IgbGridLiteColumn>` elements instead
