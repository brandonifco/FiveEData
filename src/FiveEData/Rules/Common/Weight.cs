namespace FiveEData.Rules.Common;

public readonly record struct Weight
{
    public Weight(decimal pounds)
    {
        if (pounds < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pounds),
                pounds,
                "Weight cannot be negative.");
        }

        Pounds = pounds;
    }

    public decimal Pounds { get; }
}
