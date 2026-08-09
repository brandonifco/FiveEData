namespace FiveEData.Rules.Spells;

/// <summary>
/// The PHB's melee/ranged spell attack split (p.202, "Attack Rolls and
/// Saving Throws") — the alternative to a <see cref="SpellDamageEffect"/>
/// resolving with a saving throw instead.
/// </summary>
public enum SpellAttackRollType
{
    Melee,
    Ranged
}
