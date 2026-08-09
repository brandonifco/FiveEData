using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Spells;

/// <summary>
/// A spell's damage-resolution mechanic: the PHB's "make an attack roll" or
/// "target makes a saving throw" split (p.202), plus damage dice. Present
/// only on a spell that actually deals damage on its own — most utility and
/// buff spells carry no <see cref="SpellDamageEffect"/> at all, and
/// per-spell secondary riders (Chill Touch's no-healing clause, Ray of
/// Frost's speed reduction, ...) stay in the citation, the same
/// "individually heterogeneous with no shared shape" call already made for
/// Battle Master maneuvers' secondary effects.
///
/// Two independent damage-amount shapes exist side by side because they
/// track genuinely different axes: <see cref="DamageByCharacterLevel"/> is
/// a cantrip's damage-die count increasing at 5th/11th/17th character level
/// (p.201, "Cantrips"); <see cref="BaseDamage"/> is a leveled spell's flat
/// damage at its own printed level, which never depends on character level
/// at all — its "At Higher Levels" spell-slot-upcast scaling is a
/// linear-in-slot-level formula that stays in the citation, the same call
/// already made for Preserve Life and Radiance of the Dawn. Exactly one of
/// the two is populated.
/// </summary>
public sealed record SpellDamageEffect
{
    public SpellDamageEffect(
        DamageTypeId? damageTypeId,
        IEnumerable<DamageTypeId>? choosableDamageTypeIds,
        SpellAttackRollType? attackRollType,
        AbilityId? savingThrowAbilityId,
        bool halfDamageOnSuccessfulSave,
        IEnumerable<SpellDamageTierGrant>? damageByCharacterLevel,
        DiceExpression? baseDamage)
    {
        bool hasFixedType = damageTypeId is not null;
        DamageTypeId[]? choosableTypes = choosableDamageTypeIds?.ToArray();
        bool hasChoosableTypes = choosableTypes is not null;

        if (hasFixedType == hasChoosableTypes)
        {
            throw new ArgumentException(
                "A spell damage effect must specify exactly one of a " +
                "fixed damage type or a list of choosable damage types.",
                nameof(damageTypeId));
        }

        if (choosableTypes is { Length: 0 })
        {
            throw new ArgumentException(
                "A spell damage effect's choosable damage types must not " +
                "be empty when specified.",
                nameof(choosableDamageTypeIds));
        }

        bool hasAttackRoll = attackRollType is not null;
        bool hasSavingThrow = savingThrowAbilityId is not null;

        if (hasAttackRoll == hasSavingThrow)
        {
            throw new ArgumentException(
                "A spell damage effect must resolve with exactly one of " +
                "an attack roll or a saving throw.",
                nameof(attackRollType));
        }

        if (halfDamageOnSuccessfulSave && !hasSavingThrow)
        {
            throw new ArgumentException(
                "Only a saving-throw damage effect can deal half damage " +
                "on a successful save.",
                nameof(halfDamageOnSuccessfulSave));
        }

        SpellDamageTierGrant[]? tiers = damageByCharacterLevel?.ToArray();
        bool hasTiers = tiers is { Length: > 0 };
        bool hasBaseDamage = baseDamage is not null;

        if (hasTiers == hasBaseDamage)
        {
            throw new ArgumentException(
                "A spell damage effect must specify exactly one of a " +
                "character-level damage progression or a flat base " +
                "damage.",
                nameof(damageByCharacterLevel));
        }

        if (hasTiers)
        {
            ValidateTiers(tiers!);
        }

        DamageTypeId = damageTypeId;
        ChoosableDamageTypeIds = choosableTypes is null
            ? null
            : Array.AsReadOnly(choosableTypes);
        AttackRollType = attackRollType;
        SavingThrowAbilityId = savingThrowAbilityId;
        HalfDamageOnSuccessfulSave = halfDamageOnSuccessfulSave;
        DamageByCharacterLevel = tiers is null
            ? Array.Empty<SpellDamageTierGrant>()
            : Array.AsReadOnly(tiers);
        BaseDamage = baseDamage;
    }

    private static void ValidateTiers(SpellDamageTierGrant[] tiers)
    {
        if (tiers[0].CharacterLevel != 1)
        {
            throw new ArgumentException(
                "A spell damage effect's first tier must be at character " +
                "level 1.",
                nameof(tiers));
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
                    nameof(tiers));
            }

            if (current.Damage.Sides != previous.Damage.Sides)
            {
                throw new ArgumentException(
                    "A spell damage effect's damage die size must stay " +
                    "the same across every character-level tier.",
                    nameof(tiers));
            }

            if (current.Damage.Count <= previous.Damage.Count)
            {
                throw new ArgumentException(
                    "A spell damage effect's damage die count must " +
                    "strictly increase across character-level tiers.",
                    nameof(tiers));
            }
        }
    }

    /// <summary>
    /// The damage type, when the spell always deals one fixed type. Null
    /// when <see cref="ChoosableDamageTypeIds"/> is populated instead.
    /// </summary>
    public DamageTypeId? DamageTypeId { get; }

    /// <summary>
    /// The damage types the caster may choose from, as in Chromatic Orb's
    /// "acid, cold, fire, lightning, poison, or thunder". Null when
    /// <see cref="DamageTypeId"/> is populated instead.
    /// </summary>
    public IReadOnlyList<DamageTypeId>? ChoosableDamageTypeIds { get; }

    public SpellAttackRollType? AttackRollType { get; }
    public AbilityId? SavingThrowAbilityId { get; }

    /// <summary>
    /// True when a successful save still takes half damage, the PHB's
    /// standard leveled-spell pattern (Burning Hands, Arms of Hadar, ...).
    /// Always false for a cantrip's save, where a success takes no damage
    /// at all, and for an attack-roll effect, where there is no save.
    /// </summary>
    public bool HalfDamageOnSuccessfulSave { get; }

    /// <summary>
    /// A cantrip's damage-die count at each character-level breakpoint.
    /// Empty when <see cref="BaseDamage"/> is populated instead.
    /// </summary>
    public IReadOnlyList<SpellDamageTierGrant> DamageByCharacterLevel { get; }

    /// <summary>
    /// A leveled spell's flat damage at its own printed level, independent
    /// of character level. Null when <see cref="DamageByCharacterLevel"/>
    /// is populated instead.
    /// </summary>
    public DiceExpression? BaseDamage { get; }
}
