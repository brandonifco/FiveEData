using FiveEData.Rules.Common;

namespace FiveEData.Rules.Spells;

/// <summary>
/// The damage a <see cref="SpellDamageEffect"/> deals once the caster
/// reaches a given character level — a cantrip's damage die count
/// increasing at 5th, 11th, and 17th level (p.201, "Cantrips").
/// </summary>
public readonly record struct SpellDamageTierGrant
{
    public SpellDamageTierGrant(int characterLevel, DiceExpression damage)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        CharacterLevel = characterLevel;
        Damage = damage;
    }

    public int CharacterLevel { get; }

    public DiceExpression Damage { get; }
}
