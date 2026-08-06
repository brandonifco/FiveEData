namespace FiveEData.Rules.Classes.EldritchInvocationsKnown.Serialization;

internal static class EldritchInvocationsKnownProgressionDetailDataMapper
{
    public static EldritchInvocationsKnownProgressionDetail Map(
        EldritchInvocationsKnownProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        EldritchInvocationsKnownGrantData[] invocationsKnownData =
            data.InvocationsKnownByLevel
            ?? throw new ArgumentException(
                "Eldritch Invocations known progression invocations known " +
                "by level is required.",
                nameof(data));

        EldritchInvocationsKnownGrant[] invocationsKnownByLevel =
            invocationsKnownData
                .Select(
                    grant => new EldritchInvocationsKnownGrant(
                        grant.CharacterLevel,
                        grant.InvocationsKnown))
                .ToArray();

        return new EldritchInvocationsKnownProgressionDetail(
            invocationsKnownByLevel);
    }
}
