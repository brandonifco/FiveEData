using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class ListedCostTests
{
    [Theory]
    [InlineData(ListedCostKind.Exact)]
    [InlineData(ListedCostKind.Minimum)]
    public void Constructor_PreservesValidValues(
        ListedCostKind kind)
    {
        var amount = new Money(250);

        var cost = new ListedCost(amount, kind);

        Assert.Equal(amount, cost.Amount);
        Assert.Equal(kind, cost.Kind);
    }

    [Fact]
    public void Constructor_RejectsZeroAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ListedCost(
                new Money(0),
                ListedCostKind.Exact));
    }

    [Fact]
    public void Constructor_RejectsUndefinedKind()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ListedCost(
                new Money(100),
                (ListedCostKind)999));
    }
}
