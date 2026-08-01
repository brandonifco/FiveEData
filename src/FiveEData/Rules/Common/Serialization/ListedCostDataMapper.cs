namespace FiveEData.Rules.Common.Serialization;

internal static class ListedCostDataMapper
{
    public static ListedCost Map(ListedCostData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        MoneyData amount = data.Amount
            ?? throw new ArgumentException(
                "Listed cost amount is required.",
                nameof(data));

        return new ListedCost(
            new Money(amount.CopperPieces),
            data.Kind);
    }
}
