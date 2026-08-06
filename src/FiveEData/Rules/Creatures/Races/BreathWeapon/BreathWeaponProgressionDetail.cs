namespace FiveEData.Rules.Creatures.Races.BreathWeapon;

public sealed record BreathWeaponProgressionDetail
{
    public BreathWeaponProgressionDetail(
        IEnumerable<BreathWeaponDamageGrant> damageByLevel,
        bool recoversOnShortRest)
    {
        ArgumentNullException.ThrowIfNull(damageByLevel);

        DamageByLevel = Array.AsReadOnly(damageByLevel.ToArray());
        RecoversOnShortRest = recoversOnShortRest;
    }

    public IReadOnlyList<BreathWeaponDamageGrant> DamageByLevel { get; }
    public bool RecoversOnShortRest { get; }
}
