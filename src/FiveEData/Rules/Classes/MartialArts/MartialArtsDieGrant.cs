using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.MartialArts;

public readonly record struct MartialArtsDieGrant
{
    public MartialArtsDieGrant(int characterLevel, DiceExpression die)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        CharacterLevel = characterLevel;
        Die = die;
    }

    public int CharacterLevel { get; }

    public DiceExpression Die { get; }
}
