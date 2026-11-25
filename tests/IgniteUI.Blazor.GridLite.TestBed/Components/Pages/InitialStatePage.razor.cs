using IgniteUI.Blazor.Controls;
using IgniteUI.Blazor.GridLite.TestBed.Components.Models;
using Microsoft.AspNetCore.Components;

namespace IgniteUI.Blazor.GridLite.TestBed.Components.Pages;

public partial class InitialStatePage : ComponentBase
{
    private readonly GridData data = new();
    private readonly IgbGridLiteSortingExpression[] sortingExpressions =
    [
        new() { Key = nameof(NwindDataItem.ProductID), Direction = GridLiteSortingDirection.Descending },
        new() { Key = nameof(NwindDataItem.ProductName), Direction = GridLiteSortingDirection.None }
    ];
    private readonly IgbGridLiteFilterExpression[] filterExpressions =
    [
        new() { Key = nameof(NwindDataItem.ProductName), Condition = "contains", SearchTerm = "Chai" }
    ];
    private bool rendered;

    private void OnRendered() => rendered = true;
}
