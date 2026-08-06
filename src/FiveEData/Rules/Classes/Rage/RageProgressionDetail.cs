using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.Rage;

public sealed record RageProgressionDetail
{
    public RageProgressionDetail(
        IEnumerable<RageUseGrant> usesByLevel,
        IEnumerable<RageDamageBonusGrant> damageBonusByLevel,
        int durationMinutes,
        IEnumerable<DamageTypeId> resistedDamageTypeIds,
        bool requiresNotWearingHeavyArmor)
    {
        ArgumentNullException.ThrowIfNull(usesByLevel);
        ArgumentNullException.ThrowIfNull(damageBonusByLevel);
        ArgumentNullException.ThrowIfNull(resistedDamageTypeIds);

        if (durationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationMinutes),
                durationMinutes,
                "Rage duration must be greater than zero.");
        }

        UsesByLevel = Array.AsReadOnly(usesByLevel.ToArray());
        DamageBonusByLevel = Array.AsReadOnly(damageBonusByLevel.ToArray());
        DurationMinutes = durationMinutes;
        ResistedDamageTypeIds =
            Array.AsReadOnly(resistedDamageTypeIds.ToArray());
        RequiresNotWearingHeavyArmor = requiresNotWearingHeavyArmor;
    }

    public IReadOnlyList<RageUseGrant> UsesByLevel { get; }
    public IReadOnlyList<RageDamageBonusGrant> DamageBonusByLevel { get; }
    public int DurationMinutes { get; }
    public IReadOnlyList<DamageTypeId> ResistedDamageTypeIds { get; }
    public bool RequiresNotWearingHeavyArmor { get; }
}
