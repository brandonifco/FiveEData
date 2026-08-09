using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Spells;

/// <summary>
/// A spell's damage-resolution mechanic: the PHB's "make an attack roll" or
/// "target makes a saving throw" split (p.202), plus the damage dice for
/// each character-level breakpoint at which a cantrip's damage die count
/// increases (p.201, "Cantrips"). Present only on a spell that actually
/// deals damage on its own — most cantrips (utility and buff effects) carry
/// no <see cref="SpellDamageEffect"/> at all, and per-spell secondary riders
/// (Chill Touch's no-healing clause, Shocking Grasp's no-reactions clause,
/// ...) stay in the citation, the same "individually heterogeneous with no
/// shared shape" call already made for Battle Master maneuvers' secondary
/// effects.
/// </summary>
public sealed record SpellDamageEffect
{
    public SpellDamageEffect(
        DamageTypeId damageTypeId,
        SpellAttackRollType? attackRollType,
        AbilityId? savingThrowAbilityId,
        IEnumerable<SpellDamageTierGrant> damageByCharacterLevel)
    {
        ArgumentNullException.ThrowIfNull(damageByCharacterLevel);

        bool hasAttackRoll = attackRollType is not null;
        bool hasSavingThrow = savingThrowAbilityId is not null;

        if (hasAttackRoll == hasSavingThrow)
        {
            throw new ArgumentException(
                "A spell damage effect must resolve with exactly one of " +
                "an attack roll or a saving throw.",
                nameof(attackRollType));
        }

        SpellDamageTierGrant[] tiers = damageByCharacterLevel.ToArray();

        if (tiers.Length == 0)
        {
            throw new ArgumentException(
                "A spell damage effect must specify damage for at least " +
                "one character level.",
                nameof(damageByCharacterLevel));
        }

        if (tiers[0].CharacterLevel != 1)
        {
            throw new ArgumentException(
                "A spell damage effect's first tier must be at character " +
                "level 1.",
                nameof(damageByCharacterLevel));
        }

        for (int index = 1; index < tiers.Length; index++)
        {
            SpellDamageTierGrant previous = tiers[index - 1];
            SpellDamageTierGrant current = tiers[index];

            if (current.CharacterLevel <= previous.CharacterLevel)
            {
                throw new ArgumentException(
                    "A spell damage effect's tiers must be in strictly " +
                    "ascending character-level order.",
                    nameof(damageByCharacterLevel));
            }

            if (current.Damage.Sides != previous.Damage.Sides)
            {
                throw new ArgumentException(
                    "A spell damage effect's damage die size must stay " +
                    "the same across every character-level tier.",
                    nameof(damageByCharacterLevel));
            }

            if (current.Damage.Count <= previous.Damage.Count)
            {
                throw new ArgumentException(
                    "A spell damage effect's damage die count must " +
                    "strictly increase across character-level tiers.",
                    nameof(damageByCharacterLevel));
            }
        }

        DamageTypeId = damageTypeId;
        AttackRollType = attackRollType;
        SavingThrowAbilityId = savingThrowAbilityId;
        DamageByCharacterLevel = Array.AsReadOnly(tiers);
    }

    public DamageTypeId DamageTypeId { get; }

    public SpellAttackRollType? AttackRollType { get; }

    public AbilityId? SavingThrowAbilityId { get; }

    public IReadOnlyList<SpellDamageTierGrant> DamageByCharacterLevel { get; }
}
