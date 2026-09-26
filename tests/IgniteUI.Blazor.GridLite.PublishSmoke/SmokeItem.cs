namespace IgniteUI.Blazor.GridLite.PublishSmoke;

// Top-level and without trim attributes on purpose: types nested in a component class are kept with it,
// which would hide exactly the trimming the smoke checks look for.
public class SmokeItem
{
    public string Name { get; set; } = "";

    public double Price { get; set; }

    public SmokeDetail Detail { get; set; } = new();
}

public class SmokeDetail
{
    public string Code { get; set; } = "";
}
