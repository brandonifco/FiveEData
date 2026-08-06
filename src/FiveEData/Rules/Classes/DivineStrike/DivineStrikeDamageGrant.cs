using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.DivineStrike;

public readonly record struct DivineStrikeDamageGrant
{
    public DivineStrikeDamageGrant(int characterLevel, DiceExpression damage)
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
