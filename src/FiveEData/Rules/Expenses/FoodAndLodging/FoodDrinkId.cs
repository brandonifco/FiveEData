namespace FiveEData.Rules.Expenses.FoodAndLodging;

public readonly record struct FoodDrinkId
{
    public FoodDrinkId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
