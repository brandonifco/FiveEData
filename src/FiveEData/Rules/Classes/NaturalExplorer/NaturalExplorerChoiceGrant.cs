namespace FiveEData.Rules.Classes.NaturalExplorer;

public readonly record struct NaturalExplorerChoiceGrant
{
    public NaturalExplorerChoiceGrant(
        int characterLevel,
        int favoredTerrainsKnown)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (favoredTerrainsKnown <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(favoredTerrainsKnown),
                favoredTerrainsKnown,
                "Favored terrains known must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        FavoredTerrainsKnown = favoredTerrainsKnown;
    }

    public int CharacterLevel { get; }

    public int FavoredTerrainsKnown { get; }
}
