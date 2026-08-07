namespace FiveEData.Rules.Classes.FastMovement;

public sealed record FastMovementDetail
{
    public FastMovementDetail(
        int speedBonusFeet,
        bool requiresNotWearingHeavyArmor)
    {
        if (speedBonusFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(speedBonusFeet),
                speedBonusFeet,
                "Fast Movement speed bonus must be greater than zero.");
        }

        SpeedBonusFeet = speedBonusFeet;
        RequiresNotWearingHeavyArmor = requiresNotWearingHeavyArmor;
    }

    public int SpeedBonusFeet { get; }

    public bool RequiresNotWearingHeavyArmor { get; }
}
