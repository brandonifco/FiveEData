namespace FiveEData.Rules.Classes.CantripsKnown;

public sealed record CantripsKnownProgressionDetail
{
    public CantripsKnownProgressionDetail(
        IEnumerable<CantripsKnownGrant> cantripsKnownByLevel)
    {
        ArgumentNullException.ThrowIfNull(cantripsKnownByLevel);

        CantripsKnownByLevel =
            Array.AsReadOnly(cantripsKnownByLevel.ToArray());
    }

    public IReadOnlyList<CantripsKnownGrant> CantripsKnownByLevel { get; }
}
