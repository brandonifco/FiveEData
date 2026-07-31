using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class ContainerVolumeTests
{
    [Fact]
    public void Constructor_PreservesPositiveAmountAndUnit()
    {
        var volume = new ContainerVolume(1.5m, ContainerVolumeUnit.Pint);

        Assert.Equal(1.5m, volume.Amount);
        Assert.Equal(ContainerVolumeUnit.Pint, volume.Unit);
    }

    [Fact]
    public void Constructor_RejectsNonPositiveAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ContainerVolume(0m, ContainerVolumeUnit.Gallon));
    }

    [Fact]
    public void Constructor_RejectsUndefinedUnit()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ContainerVolume(1m, (ContainerVolumeUnit)999));
    }
}
