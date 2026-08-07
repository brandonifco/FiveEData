namespace FiveEData.Rules.Classes.NaturalExplorer.Serialization;

internal static class NaturalExplorerProgressionDetailDataMapper
{
    public static NaturalExplorerProgressionDetail Map(
        NaturalExplorerProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        NaturalExplorerChoiceGrantData[] grantData =
            data.FavoredTerrainsKnownByLevel
            ?? throw new ArgumentException(
                "Natural Explorer progression favored terrains known by " +
                "level is required.",
                nameof(data));

        NaturalExplorerChoiceGrant[] favoredTerrainsKnownByLevel = grantData
            .Select(MapGrant)
            .ToArray();

        return new NaturalExplorerProgressionDetail(
            favoredTerrainsKnownByLevel);
    }

    private static NaturalExplorerChoiceGrant MapGrant(
        NaturalExplorerChoiceGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new NaturalExplorerChoiceGrant(
            data.CharacterLevel,
            data.FavoredTerrainsKnown);
    }
}
