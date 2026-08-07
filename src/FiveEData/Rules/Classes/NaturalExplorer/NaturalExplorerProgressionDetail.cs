namespace FiveEData.Rules.Classes.NaturalExplorer;

public sealed record NaturalExplorerProgressionDetail
{
    public NaturalExplorerProgressionDetail(
        IEnumerable<NaturalExplorerChoiceGrant> favoredTerrainsKnownByLevel)
    {
        ArgumentNullException.ThrowIfNull(favoredTerrainsKnownByLevel);

        FavoredTerrainsKnownByLevel =
            Array.AsReadOnly(favoredTerrainsKnownByLevel.ToArray());
    }

    public IReadOnlyList<NaturalExplorerChoiceGrant>
        FavoredTerrainsKnownByLevel
    { get; }
}
