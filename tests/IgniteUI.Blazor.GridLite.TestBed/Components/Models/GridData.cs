namespace IgniteUI.Blazor.GridLite.TestBed.Components.Models;

public class NwindDataItem
{
    public double ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
    public double UnitsInStock { get; set; }
    public bool Discontinued { get; set; }
}

/// <summary>
/// Static sample rows for the integration test pages (trimmed from the demo's NwindData).
/// </summary>
public class GridData : List<NwindDataItem>
{
    public GridData()
    {
        Add(new() { ProductID = 1, ProductName = "Chai", UnitPrice = 18, UnitsInStock = 39, Discontinued = false });
        Add(new() { ProductID = 2, ProductName = "Chang", UnitPrice = 19, UnitsInStock = 17, Discontinued = true });
        Add(new() { ProductID = 3, ProductName = "Aniseed Syrup", UnitPrice = 10, UnitsInStock = 13, Discontinued = false });
        Add(new() { ProductID = 4, ProductName = "Chef Antons Cajun Seasoning", UnitPrice = 22, UnitsInStock = 53, Discontinued = false });
        Add(new() { ProductID = 5, ProductName = "Chef Antons Gumbo Mix", UnitPrice = 21.35, UnitsInStock = 0, Discontinued = true });
        Add(new() { ProductID = 6, ProductName = "Grandmas Boysenberry Spread", UnitPrice = 25, UnitsInStock = 0, Discontinued = false });
        Add(new() { ProductID = 7, ProductName = "Uncle Bobs Organic Dried Pears", UnitPrice = 30, UnitsInStock = 150, Discontinued = false });
        Add(new() { ProductID = 8, ProductName = "Northwoods Cranberry Sauce", UnitPrice = 40, UnitsInStock = 6, Discontinued = false });
    }
}
