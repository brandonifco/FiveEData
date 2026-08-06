using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.DivineStrike;

public sealed record DivineStrikeProgressionDetail
{
    public DivineStrikeProgressionDetail(
        IEnumerable<DivineStrikeDamageGrant> damageByLevel,
        DamageTypeId? fixedDamageTypeId,
        IEnumerable<DamageTypeId>? choosableDamageTypeIds,
        bool matchesWeaponDamageType)
    {
        ArgumentNullException.ThrowIfNull(damageByLevel);

        DamageByLevel = Array.AsReadOnly(damageByLevel.ToArray());
        FixedDamageTypeId = fixedDamageTypeId;
        ChoosableDamageTypeIds = choosableDamageTypeIds is null
            ? null
            : Array.AsReadOnly(choosableDamageTypeIds.ToArray());
        MatchesWeaponDamageType = matchesWeaponDamageType;
    }

    public IReadOnlyList<DivineStrikeDamageGrant> DamageByLevel { get; }
    public DamageTypeId? FixedDamageTypeId { get; }
    public IReadOnlyList<DamageTypeId>? ChoosableDamageTypeIds { get; }
    public bool MatchesWeaponDamageType { get; }
}
