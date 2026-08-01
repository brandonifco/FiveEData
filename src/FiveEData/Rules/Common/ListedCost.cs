namespace FiveEData.Rules.Common;

public readonly record struct ListedCost
{
    public ListedCost(
        Money amount,
        ListedCostKind kind)
    {
        if (amount.CopperPieces <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Listed cost must be greater than zero.");
        }

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(kind),
                kind,
                "Listed cost kind must be defined.");
        }

        Amount = amount;
        Kind = kind;
    }

    public Money Amount { get; }
    public ListedCostKind Kind { get; }
}
