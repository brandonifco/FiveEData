namespace FiveEData.Rules.Classes.UnarmoredMovement;

public sealed record UnarmoredMovementProgressionDetail
{
    public UnarmoredMovementProgressionDetail(
        IEnumerable<UnarmoredMovementSpeedBonusGrant> speedBonusByLevel,
        bool requiresNotWearingArmor,
        bool requiresNotWieldingShield)
    {
        ArgumentNullException.ThrowIfNull(speedBonusByLevel);

        SpeedBonusByLevel = Array.AsReadOnly(speedBonusByLevel.ToArray());
        RequiresNotWearingArmor = requiresNotWearingArmor;
        RequiresNotWieldingShield = requiresNotWieldingShield;
    }

    public IReadOnlyList<UnarmoredMovementSpeedBonusGrant> SpeedBonusByLevel
    {
        get;
    }

    public bool RequiresNotWearingArmor { get; }

    public bool RequiresNotWieldingShield { get; }
}
