namespace FiveEData.Rules.Equipment.TradeGoods;

public readonly record struct TradeGoodId
{
    public TradeGoodId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
