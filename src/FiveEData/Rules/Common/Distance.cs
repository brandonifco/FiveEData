namespace FiveEData.Rules.Common;

public readonly record struct Distance
{
    public Distance(int feet)
    {
        if (feet < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(feet),
                feet,
                "Distance cannot be negative.");
        }

        Feet = feet;
    }

    public int Feet { get; }
}
