# Ignite UI GridLite Blazor Wrapper

A Blazor wrapper for the [Ignite UI GridLite](https://www.npmjs.com/package/igniteui-grid-lite) web component, providing a lightweight and performant data grid for Blazor applications.

## Features

- 🚀 Lightweight grid component built on web components
- 📊 Column configuration with custom headers
- 🔄 Sorting and filtering support
- 🎨 Multiple built-in themes (Bootstrap, Material, Fluent, Indigo) with light/dark variants
- 🔌 Easy integration with existing Blazor applications
- 🎯 Multi-framework support (.NET 8, 9, and 10)

## Installation

Install the NuGet package:

```bash
dotnet add package IgniteUI.Blazor.GridLite
```

## Setup

### 1. Add Required Services (TBD)

In your `Program.cs`, no special services are required. The component uses standard Blazor JSInterop.

### 2. Include JavaScript Module

The JavaScript bundle is automatically included via `_content` static files.

### 3. Add Theme Stylesheet

In your `App.razor` or layout file, include one of the available themes:

```html
<!-- Light themes -->
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/bootstrap.css" rel="stylesheet" />
<!-- or -->
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/material.css" rel="stylesheet" />
<!-- or -->
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/fluent.css" rel="stylesheet" />
<!-- or -->
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/indigo.css" rel="stylesheet" />

<!-- Dark themes also available in css/themes/dark/ -->
```

## Basic Usage

```razor
@using IgniteUI.Blazor.Controls

<IgbGridLite Data="@employees">
    <IgbGridLiteColumn Field="Id" Header="ID" DataType="GridLiteColumnDataType.Number" Width="100px" />
    <IgbGridLiteColumn Field="Name" Header="Employee Name" DataType="GridLiteColumnDataType.String" />
    <IgbGridLiteColumn Field="Department" Header="Department" DataType="GridLiteColumnDataType.String" />
    <IgbGridLiteColumn Field="Salary" Header="Salary" DataType="GridLiteColumnDataType.Number" Width="150px" />
</IgbGridLite>

@code {
    private List<Employee> employees = new();

    protected override void OnInitialized()
    {
        employees = GetEmployees();
    }
}
```

### With Initial Sort and Filter

```razor
<IgbGridLite Data="@employees"
             SortingExpressions="@initialSort"
             FilterExpressions="@initialFilter">
    <IgbGridLiteColumn Field="Id" Header="ID" DataType="GridLiteColumnDataType.Number" />
    <IgbGridLiteColumn Field="Name" Header="Name" Sortable Filterable />
    <IgbGridLiteColumn Field="Department" Header="Department" Sortable Filterable />
</IgbGridLite>

@code {
    private List<IgbGridLiteSortingExpression> initialSort = new()
    {
        new() { Key = nameof(Employee.Name), Direction = GridLiteSortingDirection.Ascending }
    };
    
    private List<IgbGridLiteFilterExpression> initialFilter = new()
    {
        new() { Key = nameof(Employee.Department), Condition = "contains", SearchTerm = "Sales" }
    };
}
```

## Advanced Configuration

### Sorting

Enable sorting on specific columns:

```razor
<IgbGridLiteColumn Field="Name" 
                   Header="Name"
                   Sortable
                   Resizable />
```

### Filtering

Enable filtering on columns:

```razor
<IgbGridLiteColumn Field="Department" 
                   Header="Department"
                   Filterable
                   FilteringCaseSensitive="@false" />
```

### Event Handling

Handle sorting and filtering events:

```razor
<IgbGridLite Data="@employees"
             Sorting="@HandleSorting"
             Sorted="@HandleSorted"
             Filtering="@HandleFiltering"
             Filtered="@HandleFiltered">
    <IgbGridLiteColumn Field="Name" Sortable Filterable />
    <IgbGridLiteColumn Field="Department" Sortable Filterable />
</IgbGridLite>

@code {
    private void HandleSorting(IgbGridLiteSortingEventArgs e)
    {
        // Handle on sorting
    }

    private void HandleSorted(IgbGridLiteSortedEventArgs e)
    {
        // Handle after sort
    }

    private void HandleFiltering(IgbGridLiteFilteringEventArgs e)
    {
        // Handle on filtering
    }

    private void HandleFiltered(IgbGridLiteFilteredEventArgs e)
    {
        // Handle after filter
    }
}
```

## Column Configuration

The `IgbGridLiteColumn` component supports:

- `Field`: Property name to bind to (use `nameof()` for type safety)
- `Header`: Column header display text
- `Width`: Column width (CSS value)
- `DataType`: Data type (String, Number, Boolean, Date)
- `Hidden`: Hide column
- `Resizable`: Allow column resizing
- `Sortable`: Enable sorting
- `SortingCaseSensitive`: Make sorting case sensitive
- `Filterable`: Enable filtering
- `FilteringCaseSensitive`: Make filtering case sensitive

## Building from Source

### Prerequisites

- .NET 8, 9, or 10 SDK
- Node.js (for building JavaScript bundle)

### Build Steps

1. Restore dependencies:
    ```bash
    dotnet restore
    ```

2. Build project:
    ```bash
    dotnet build
    ```

The build process (configured in `IgniteUI.Blazor.GridLite.csproj`) automatically:
- Installs npm dependencies
- Builds the JavaScript bundle using Vite
- Copies theme files to wwwroot

## Demo Application

A demo application is available in `demo/GridLite.DemoApp/` showcasing various grid features and configurations.
