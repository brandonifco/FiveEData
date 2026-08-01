using FiveEData.Rules.Equipment.TradeGoods;

namespace FiveEData.Tests;

public sealed class TradeGoodPricingBasisTests
{
    [Fact]
    public void ValidPricingBasis_PreservesQuantityAndUnit()
    {
        var basis = new TradeGoodPricingBasis(
            1.5m,
            TradeGoodUnit.SquareYard);

        Assert.Equal(1.5m, basis.Quantity);
        Assert.Equal(TradeGoodUnit.SquareYard, basis.Unit);
    }

    [Fact]
    public void ZeroQuantity_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new TradeGoodPricingBasis(
                0,
                TradeGoodUnit.Pound));
    }

    [Fact]
    public void UndefinedUnit_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new TradeGoodPricingBasis(
                1,
                (TradeGoodUnit)999));
    }
}
