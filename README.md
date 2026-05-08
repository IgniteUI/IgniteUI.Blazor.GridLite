# Ignite UI Blazor Grid Lite

[![NuGet version](https://img.shields.io/nuget/v/IgniteUI.Blazor.GridLite.svg)](https://www.nuget.org/packages/IgniteUI.Blazor.GridLite)
[![NuGet downloads](https://img.shields.io/nuget/dt/IgniteUI.Blazor.GridLite.svg)](https://www.nuget.org/packages/IgniteUI.Blazor.GridLite)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Discord](https://img.shields.io/discord/836634487483269200?logo=discord&logoColor=ffffff)](https://discord.gg/39MjrTRqds)

`IgniteUI.Blazor.GridLite` is a Blazor wrapper from Infragistics around the [Ignite UI Grid Lite](https://github.com/IgniteUI/igniteui-grid-lite) web component. It provides a lightweight, MIT-licensed data grid for Blazor Server and Blazor WebAssembly applications, with column configuration via Razor components, sorting and filtering support, and four built-in themes (Material, Bootstrap, Fluent, Indigo) in light and dark variants. It targets .NET 8, .NET 9, and .NET 10.

This package is the Blazor wrapper for Grid Lite only. It does not include cell editing, grouping, aggregation, paging, advanced filtering, or Excel/PDF export. For those features, use the commercial [`IgniteUI.Blazor`](https://www.nuget.org/packages/IgniteUI.Blazor) package, which includes the full [Ignite UI for Blazor Data Grid](https://www.infragistics.com/products/ignite-ui-blazor/blazor/components/grids/data-grid). For the underlying web component used in non-Blazor frameworks, see [`igniteui-grid-lite`](https://github.com/IgniteUI/igniteui-grid-lite).

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Setup](#setup)
- [Basic Usage](#basic-usage)
- [Advanced Configuration](#advanced-configuration)
- [Column Configuration Reference](#column-configuration-reference)
- [AI-Assisted Development](#ai-assisted-development)
- [Building from Source](#building-from-source)
- [Demo Application](#demo-application)
- [Support](#support)
- [Contributing](#contributing)
- [License](#license)

## Features

- 🚀 Lightweight grid component built on web components
- 📊 Column configuration with custom headers
- 🔄 Sorting and filtering support
- 🎨 Multiple built-in themes (Bootstrap, Material, Fluent, Indigo) with light/dark variants
- 🔌 Easy integration with existing Blazor Server and Blazor WebAssembly applications
- 🎯 Multi-framework support (.NET 8, 9, and 10)

## Installation

Install the NuGet package:

```bash
dotnet add package IgniteUI.Blazor.GridLite
```

## Setup

1 - Add the **IgniteUI.Blazor.Controls** namespace in the **_Imports.razor** file:

```razor
@using IgniteUI.Blazor.Controls
```

2 - Add the Style Sheet in the appropriate location based on your project type:

```razor
<head>
       <link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/bootstrap.css" rel="stylesheet" />
</head>
```

3 - Add the Grid Lite component to your razor page:

```razor
<IgbGridLite Data="data" AutoGenerateColumns="true">
</IgbGridLite>

@code {
    private object[] data = new object[]
    {
        new { Name = "John", Age = 30, City = "New York" },
        new { Name = "Jane", Age = 25, City = "Los Angeles" },
        new { Name = "Bob", Age = 35, City = "Chicago" }
    };
}
```


	  

										   

			  

		
### Include the Theme Stylesheet

Add one theme to your `App.razor`, `_Layout.cshtml`, or main layout file. Each theme is available in light and dark variants.

```html
<!-- Light themes -->
																																 
			  
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/material.css" rel="stylesheet" />

<!-- Or one of: -->
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/bootstrap.css" rel="stylesheet" />
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/fluent.css" rel="stylesheet" />
	 
<link href="_content/IgniteUI.Blazor.GridLite/css/themes/light/indigo.css" rel="stylesheet" />

<!-- Dark variants are available under css/themes/dark/ with the same names -->
```

### JavaScript Module

The JavaScript bundle is automatically included via `_content` static files - no manual registration is required.

### Service Registration

No service registration is required. The component uses standard Blazor JSInterop and is ready to use after installing the NuGet package and including the theme stylesheet.

## Basic Usage

```razor
@using IgniteUI.Blazor.Controls

<IgbGridLite Data="@employees">
    <IgbGridLiteColumn Field="@nameof(Employee.Id)" Header="ID" DataType="GridLiteColumnDataType.Number" Width="100px" />
    <IgbGridLiteColumn Field="@nameof(Employee.Name)" Header="Employee Name" DataType="GridLiteColumnDataType.String" />
    <IgbGridLiteColumn Field="@nameof(Employee.Department)" Header="Department" DataType="GridLiteColumnDataType.String" />
    <IgbGridLiteColumn Field="@nameof(Employee.Salary)" Header="Salary" DataType="GridLiteColumnDataType.Number" Width="150px" />
</IgbGridLite>

@code {
    private List<Employee> employees = new();

    protected override void OnInitialized()
    {
        employees = GetEmployees();
    }
}
```

> **Note:** The `Salary` column above uses `DataType.Number` for clarity. For currency formatting, render the value through a `CellTemplate` and apply the formatter on the rendered text rather than at the column-type level.

## Advanced Configuration

### Initial Sort and Filter State

Configure pre-applied sorting and filtering when the grid first renders:

```razor
<IgbGridLite Data="@employees"
             SortingExpressions="@initialSort"
             FilterExpressions="@initialFilter">
    <IgbGridLiteColumn Field="@nameof(Employee.Id)" Header="ID" DataType="GridLiteColumnDataType.Number" />
    <IgbGridLiteColumn Field="@nameof(Employee.Name)" Header="Name" Sortable Filterable />
    <IgbGridLiteColumn Field="@nameof(Employee.Department)" Header="Department" Sortable Filterable />
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

									 
								   
### Sorting

Enable sorting on individual columns and optionally make sorting case-sensitive:

```razor
<IgbGridLiteColumn Field="@nameof(Employee.Name)"
                   Header="Name"
                   Sortable
                   Resizable />
```

### Filtering

Enable filtering with optional case-sensitivity:

```razor
<IgbGridLiteColumn Field="Department"
                   Header="Department"
                   Filterable
                   FilteringCaseSensitive="@false" />
```

### Event Handling

Handle sorting and filtering events to persist user state, log analytics, or react to grid interactions:

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
        // Fires before the sort is applied. Set e.Cancel = true to prevent the sort.
    }

    private void HandleSorted(IgbGridLiteSortedEventArgs e)
    {
        // Fires after the sort is applied. Persist current sort state to a user profile.
        UserPreferences.LastGridSort = (e.Key, e.Direction);
    }

    private void HandleFiltering(IgbGridLiteFilteringEventArgs e)
    {
        // Fires before the filter is applied.
    }

    private void HandleFiltered(IgbGridLiteFilteredEventArgs e)
    {
        // Fires after the filter is applied. Log the filter for analytics.
        Analytics.Track("grid.filter.applied", new { e.Key, e.Condition, e.SearchTerm });
    }
}
```

## Column Configuration Reference

The `IgbGridLiteColumn` component supports the following properties:

| Property                 | Type                          | Description                                                                                  |
| :----------------------- | :---------------------------- | :------------------------------------------------------------------------------------------- |
| `Field`                  | `string`                      | The model property to bind to. Use `nameof()` for compile-time safety.                       |
| `Header`                 | `string`                      | Column header display text.                                                                  |
| `Width`                  | `string`                      | Column width as a CSS value (e.g., `"100px"`, `"20%"`, `"auto"`).                            |
| `DataType`               | `GridLiteColumnDataType`      | One of `String`, `Number`, `Boolean`, or `Date`.                                             |
| `Hidden`                 | `bool`                        | Hides the column when `true`.                                                                |
| `Resizable`              | `bool`                        | Allows the user to resize the column.                                                        |
| `Sortable`               | `bool`                        | Enables sorting on the column.                                                               |
| `SortingCaseSensitive`   | `bool`                        | When `true`, sort comparisons are case-sensitive.                                            |
| `Filterable`             | `bool`                        | Enables filtering on the column.                                                             |
| `FilteringCaseSensitive` | `bool`                        | When `true`, filter comparisons are case-sensitive.                                          |

## AI-Assisted Development

Ignite UI ships an AI toolchain that grounds AI coding assistants (GitHub Copilot, Cursor, Windsurf, Claude Desktop, Claude Code, JetBrains AI Assistant) in correct component APIs and theming patterns. For Blazor today, the toolchain provides:

- **Ignite UI Theming MCP** (`igniteui-theming`) - palettes, design tokens, and component theming via MCP. Works with all four built-in themes.
- **MAKER Framework** (`@igniteui/maker-mcp`, optional, advanced) - multi-agent orchestration for long-horizon tasks.

Agent Skills and the Ignite UI CLI MCP do not currently support Blazor and are roadmapped for a future release. To use the available Theming MCP today, add the following block to your AI client configuration:

**VS Code (`.vscode/mcp.json`):**

```json
{
  "servers": {
    "igniteui-theming": {
      "command": "npx",
      "args": ["-y", "igniteui-theming", "igniteui-theming-mcp"]
    }
  }
}
```

**Cursor, Claude Desktop, Claude Code, JetBrains, and other MCP clients (`mcpServers` block):**

```json
{
  "mcpServers": {
    "igniteui-theming": {
      "command": "npx",
      "args": ["-y", "igniteui-theming", "igniteui-theming-mcp"]
    }
  }
}
```

For the full setup guide and configuration options, see the [Ignite UI Theming MCP documentation](https://www.infragistics.com/products/ignite-ui-blazor/blazor/components/ai/theming-mcp).

## Building from Source

### Prerequisites

- .NET 8, .NET 9, or .NET 10 SDK.
- Node.js 18 or later (used to build the JavaScript bundle).

### Build Steps

Restore dependencies:

							  
					  

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

The build process (configured in `IgniteUI.Blazor.GridLite.csproj`) automatically:
					  

		

1. Installs npm dependencies.
2. Builds the JavaScript bundle using Vite.
3. Copies theme files to `wwwroot`.

## Demo Application

A demo application is available in [`demo/GridLite.DemoApp/`](demo/GridLite.DemoApp/) showcasing the supported grid features and configurations.

## Support

### Community Support

- [GitHub Issues](https://github.com/IgniteUI/IgniteUI.Blazor.GridLite/issues) - bug reports and feature requests.
- [Infragistics Discord](https://discord.gg/39MjrTRqds) - real-time discussion.
- [Stack Overflow](https://stackoverflow.com/questions/tagged/igniteui) - tag `igniteui`.

### Commercial Support

For 24/5 developer support, an SLA, and access to the full [Ignite UI for Blazor](https://www.infragistics.com/products/ignite-ui-blazor) suite (commercial Data Grid, Tree Grid, Hierarchical Grid, Pivot Grid, charts, gauges, maps, Dock Manager, Spreadsheet, Excel library):

- [Infragistics Support](https://www.infragistics.com/support)
- [Ignite UI for Blazor product page](https://www.infragistics.com/products/ignite-ui-blazor)
- [Pricing and license options](https://www.infragistics.com/how-to-buy/product-pricing#developers)

## Contributing

Contributions are welcome. See the [Contribution Guide](.github/CONTRIBUTING.md) for:

- Development workflow.
- Coding standards.
- Pull request submission.

To contribute:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature`.
3. Commit your changes: `git commit -m 'Add your feature'`.
4. Push the branch: `git push origin feature/your-feature`.
5. Open a Pull Request.

## License

This project is MIT-licensed. See [LICENSE](LICENSE) for details.

© Copyright 2026 INFRAGISTICS. All Rights Reserved.

For the commercial Ignite UI for Blazor product, see the [Infragistics Licensing page](https://www.infragistics.com/legal/license).

---

**Built by [Infragistics](https://www.infragistics.com/)**
