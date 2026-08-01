namespace FiveEData.Rules.Equipment.TradeGoods;

public readonly record struct TradeGoodPricingBasis
{
    public TradeGoodPricingBasis(
        decimal quantity,
        TradeGoodUnit unit)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "Trade-good pricing quantity must be greater than zero.");
        }

        if (!Enum.IsDefined(unit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(unit),
                unit,
                "Trade-good pricing unit must be defined.");
        }

        Quantity = quantity;
        Unit = unit;
    }

    public decimal Quantity { get; }
    public TradeGoodUnit Unit { get; }
}
