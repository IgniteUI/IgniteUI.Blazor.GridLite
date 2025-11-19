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

<IgbGridLite Data="@employees" 
             Columns="@columns" />

@code {
    private List<Employee> employees = new();
    private List<IgbColumnConfiguration> columns = new();

    protected override void OnInitialized()
    {
        employees = GetEmployees();
        
        columns = new List<IgbColumnConfiguration>
        {
            new() { Key = nameof(Employee.Id), HeaderText = "ID", Width = "100px", Type = GridLiteColumnDataType.Number },
            new() { Key = nameof(Employee.Name), HeaderText = "Employee Name", Type = GridLiteColumnDataType.String },
            new() { Key = nameof(Employee.Department), HeaderText = "Department", Type = GridLiteColumnDataType.String },
            new() { Key = nameof(Employee.Salary), HeaderText = "Salary", Width = "150px", Type = GridLiteColumnDataType.Number }
        };
    }
}
```

### With Initial Sort and Filter

```razor
<IgbGridLite Data="@employees" 
             Columns="@columns"
             SortExpressions="@initialSort"
             FilterExpressions="@initialFilter" />

@code {
    private List<IgbGridLiteSortExpression> initialSort = new()
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

```csharp
new IgbColumnConfiguration
{ 
    Key = nameof(Employee.Name), 
    HeaderText = "Name",
    Resizable = true,
    Sort = true // Enable sorting
}
```

### Filtering

Enable filtering on columns:

```csharp
new IgbColumnConfiguration
{ 
    Key = nameof(Employee.Department), 
    HeaderText = "Department",
    Filter = new IgbColumnFilterConfiguration 
    { 
        CaseSensitive = false 
    }
}
```

### Event Handling

Handle sorting and filtering events:

```razor
<IgbGridLite Data="@employees" 
             Columns="@columns"
             Sorting="@HandleSorting"
             Sorted="@HandleSorted"
             Filtering="@HandleFiltering"
             Filtered="@HandleFiltered" />

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

The `IgbColumnConfiguration` class supports:

- `Key`: Property name to bind to (use `nameof()` for type safety)
- `HeaderText`: Column header display text
- `Width`: Column width (CSS value)
- `Type`: Data type (String, Number, Boolean, Date)
- `Hidden`: Hide column
- `Resizable`: Allow column resizing
- `Sort`: Enable/configure sorting
- `Filter`: Enable/configure filtering

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
