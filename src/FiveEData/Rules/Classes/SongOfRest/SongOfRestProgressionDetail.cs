namespace FiveEData.Rules.Classes.SongOfRest;

public sealed record SongOfRestProgressionDetail
{
    public SongOfRestProgressionDetail(
        IEnumerable<SongOfRestDieGrant> dieByLevel)
    {
        ArgumentNullException.ThrowIfNull(dieByLevel);

        DieByLevel = Array.AsReadOnly(dieByLevel.ToArray());
    }

    public IReadOnlyList<SongOfRestDieGrant> DieByLevel { get; }
}
