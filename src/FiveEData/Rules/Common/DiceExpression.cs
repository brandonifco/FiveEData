namespace FiveEData.Rules.Common;

public readonly record struct DiceExpression
{
    public DiceExpression(int count, int sides)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                "Dice count must be greater than zero.");
        }

        if (sides <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sides),
                sides,
                "Die sides must be greater than zero.");
        }

        Count = count;
        Sides = sides;
    }

    public int Count { get; }

    public int Sides { get; }

    public override string ToString() => $"{Count}d{Sides}";
}
