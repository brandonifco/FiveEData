using FiveEData.Rules.Common;

namespace FiveEData.Rules.Creatures.Races.BreathWeapon;

public readonly record struct BreathWeaponDamageGrant
{
    public BreathWeaponDamageGrant(int characterLevel, DiceExpression damage)
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
