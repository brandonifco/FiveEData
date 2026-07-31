using FiveEData.Rules.Equipment;

namespace FiveEData.Tests;

public sealed class EquipmentChangeDurationTests
{
    [Fact]
    public void Duration_PreservesAmountAndUnit()
    {
        var duration = new EquipmentChangeDuration(
            5,
            EquipmentChangeTimeUnit.Minute);

        Assert.Equal(5, duration.Amount);
        Assert.Equal(EquipmentChangeTimeUnit.Minute, duration.Unit);
    }

    [Fact]
    public void Duration_RejectsNonPositiveAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new EquipmentChangeDuration(
                0,
                EquipmentChangeTimeUnit.Minute));
    }

    [Fact]
    public void Duration_RejectsUndefinedUnit()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new EquipmentChangeDuration(
                1,
                (EquipmentChangeTimeUnit)999));
    }
}
