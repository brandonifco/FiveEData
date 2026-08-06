namespace FiveEData.Rules.Classes.EldritchInvocationsKnown;

public sealed record EldritchInvocationsKnownProgressionDetail
{
    public EldritchInvocationsKnownProgressionDetail(
        IEnumerable<EldritchInvocationsKnownGrant> invocationsKnownByLevel)
    {
        ArgumentNullException.ThrowIfNull(invocationsKnownByLevel);

        InvocationsKnownByLevel =
            Array.AsReadOnly(invocationsKnownByLevel.ToArray());
    }

    public IReadOnlyList<EldritchInvocationsKnownGrant>
        InvocationsKnownByLevel
    { get; }
}
