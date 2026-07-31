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

    [Fact]
    public void Timing_PreservesDonAndDoffDurations()
    {
        var don = new EquipmentChangeDuration(
            5,
            EquipmentChangeTimeUnit.Minute);
        var doff = new EquipmentChangeDuration(
            1,
            EquipmentChangeTimeUnit.Minute);

        var timing = new EquipmentChangeTiming(don, doff);

        Assert.Equal(don, timing.Don);
        Assert.Equal(doff, timing.Doff);
    }

    [Fact]
    public void Timing_RejectsDefaultDonDuration()
    {
        var valid = new EquipmentChangeDuration(
            1,
            EquipmentChangeTimeUnit.Minute);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new EquipmentChangeTiming(default, valid));
    }

    [Fact]
    public void Timing_RejectsDefaultDoffDuration()
    {
        var valid = new EquipmentChangeDuration(
            1,
            EquipmentChangeTimeUnit.Minute);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new EquipmentChangeTiming(valid, default));
    }
}
