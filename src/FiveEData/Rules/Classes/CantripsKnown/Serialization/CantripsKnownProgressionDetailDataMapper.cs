namespace FiveEData.Rules.Classes.CantripsKnown.Serialization;

internal static class CantripsKnownProgressionDetailDataMapper
{
    public static CantripsKnownProgressionDetail Map(
        CantripsKnownProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        CantripsKnownGrantData[] cantripsKnownData =
            data.CantripsKnownByLevel
            ?? throw new ArgumentException(
                "Cantrips known progression cantrips known by level is " +
                "required.",
                nameof(data));

        CantripsKnownGrant[] cantripsKnownByLevel =
            cantripsKnownData
                .Select(
                    grant => new CantripsKnownGrant(
                        grant.CharacterLevel,
                        grant.CantripsKnown))
                .ToArray();

        return new CantripsKnownProgressionDetail(cantripsKnownByLevel);
    }
}
