# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Changed - Breaking Changes

This release updates the Blazor wrapper to match the API changes from `igniteui-grid-lite` version `0.3.1`.

#### From igniteui-grid-lite 0.3.1
- **Removed `UpdateColumnsAsync` method** - Columns can now be updated directly through property setters. Use parameter binding or `updateGrid` instead.

#### From igniteui-grid-lite 0.3.0 - Column Property Renames
- `IgbColumnConfiguration.Key` → `Field` - The field from the data that the column references
- `IgbColumnConfiguration.Type` → `DataType` - The data type of the column's values  
- `IgbColumnConfiguration.HeaderText` → `Header` - The header text of the column

#### From igniteui-grid-lite 0.2.0 - Sort/Filter Property Changes
- Column `Sort` property (object) replaced with:
  - `Sortable` (bool) - Enable/disable sorting
  - `SortingCaseSensitive` (bool?) - Configure case-sensitive sorting
- Column `Filter` property (object) replaced with:
  - `Filterable` (bool) - Enable/disable filtering
  - `FilteringCaseSensitive` (bool?) - Configure case-sensitive filtering
- Removed `IgbColumnFilterConfiguration` type (use `FilteringCaseSensitive` boolean directly)
- Removed `IgbColumnSortConfiguration` type (use `Sortable` and `SortingCaseSensitive` directly)

#### From igniteui-grid-lite 0.1.0 - Sorting API Renames
- `IgbGridLiteSortConfiguration` type → `IgbGridLiteSortingOptions`
- `IgbGridLite.SortConfiguration` property → `SortingOptions`
- `IgbGridLite.SortExpressions` property → `SortingExpressions`
- `IgbGridLiteSortExpression` type → `IgbGridLiteSortingExpression`
- `IgbGridLiteSortingExpression.Key` property → `Field`
- `IgbGridLiteFilterExpression.Key` property → `Field`
- `IgbGridLiteSortingOptions.Multiple` boolean → `Mode` property (accepts `"single"` or `"multiple"`)
- Removed `TriState` property (tri-state sorting is always enabled)
- Updated event args:
  - `IgbGridLiteSortingEventArgs.Expression` now uses `IgbGridLiteSortingExpression` type
  - `IgbGridLiteSortedEventArgs.Expression` now uses `IgbGridLiteSortingExpression` type

### Migration Guide

#### Updating Column Configurations

**Before:**
```csharp
new IgbColumnConfiguration {
    Key = nameof(Product.Name),
    Type = GridLiteColumnDataType.String,
    HeaderText = "Product Name",
    Sort = true,
    Filter = new IgbColumnFilterConfiguration { CaseSensitive = false }
}
```

**After:**
```csharp
new IgbColumnConfiguration {
    Field = nameof(Product.Name),
    DataType = GridLiteColumnDataType.String,
    Header = "Product Name",
    Sortable = true,
    Filterable = true,
    FilteringCaseSensitive = false
}
```

#### Updating Sort Configuration

**Before:**
```csharp
private IgbGridLiteSortConfiguration sortConfig = new() {
    Multiple = true,
    TriState = false
};

<IgbGridLite SortConfiguration="@sortConfig" />
```

**After:**
```csharp
private IgbGridLiteSortingOptions sortingOptions = new() {
    Mode = "multiple"
};

<IgbGridLite SortingOptions="@sortingOptions" />
```

#### Updating Sort Expressions

**Before:**
```csharp
private List<IgbGridLiteSortExpression> sortExpressions = new() {
    new() { Key = nameof(Product.Name), Direction = GridLiteSortingDirection.Ascending }
};

<IgbGridLite SortExpressions="@sortExpressions" />
```

**After:**
```csharp
private List<IgbGridLiteSortingExpression> sortingExpressions = new() {
    new() { Field = nameof(Product.Name), Direction = GridLiteSortingDirection.Ascending }
};

<IgbGridLite SortingExpressions="@sortingExpressions" />
```

#### Updating Filter Expressions

**Before:**
```csharp
new IgbGridLiteFilterExpression {
    Key = nameof(Product.Department),
    Condition = "contains",
    SearchTerm = "Sales"
}
```

**After:**
```csharp
new IgbGridLiteFilterExpression {
    Field = nameof(Product.Department),
    Condition = "contains",
    SearchTerm = "Sales"
}
```

#### Updating Method Calls

**Before:**
```csharp
await grid.UpdateColumnsAsync(newColumns);
await grid.SortAsync(new IgbGridLiteSortExpression { ... });
```

**After:**
```csharp
// Columns can be updated via parameter binding or updateGrid
grid.Columns = newColumns;
await grid.SortAsync(new IgbGridLiteSortingExpression { ... });
```

### Dependencies
- Updated `igniteui-grid-lite` from `~0.0.1` to `~0.3.1`
