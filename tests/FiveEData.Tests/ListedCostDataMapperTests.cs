using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Tests;

public sealed class ListedCostDataMapperTests
{
    [Theory]
    [InlineData(ListedCostKind.Exact)]
    [InlineData(ListedCostKind.Minimum)]
    public void Map_PreservesValidValues(ListedCostKind kind)
    {
        var data = new ListedCostData
        {
            Amount = new MoneyData
            {
                CopperPieces = 250
            },
            Kind = kind
        };

        ListedCost cost = ListedCostDataMapper.Map(data);

        Assert.Equal(250, cost.Amount.CopperPieces);
        Assert.Equal(kind, cost.Kind);
    }

    [Fact]
    public void Map_RejectsMissingAmount()
    {
        var data = new ListedCostData
        {
            Amount = null,
            Kind = ListedCostKind.Exact
        };

        Assert.Throws<ArgumentException>(
            () => ListedCostDataMapper.Map(data));
    }

    [Fact]
    public void Map_RejectsZeroAmount()
    {
        var data = new ListedCostData
        {
            Amount = new MoneyData
            {
                CopperPieces = 0
            },
            Kind = ListedCostKind.Exact
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => ListedCostDataMapper.Map(data));
    }

    [Fact]
    public void Map_RejectsUndefinedKind()
    {
        var data = new ListedCostData
        {
            Amount = new MoneyData
            {
                CopperPieces = 100
            },
            Kind = (ListedCostKind)999
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => ListedCostDataMapper.Map(data));
    }
}
