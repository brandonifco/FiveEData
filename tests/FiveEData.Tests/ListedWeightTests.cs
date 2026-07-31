using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class ListedWeightTests
{
    [Fact]
    public void Constructor_PreservesWeightAndQualifier()
    {
        var listedWeight = new ListedWeight(new Weight(5m), "full");

        Assert.Equal(5m, listedWeight.Weight.Pounds);
        Assert.Equal("full", listedWeight.Qualifier);
    }

    [Fact]
    public void Constructor_RejectsNonPositiveWeight()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ListedWeight(default));
    }

    [Fact]
    public void Constructor_RejectsBlankQualifier()
    {
        Assert.Throws<ArgumentException>(
            () => new ListedWeight(new Weight(1m), " "));
    }
}
