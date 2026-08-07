namespace FiveEData.Rules.Classes.Indomitable.Serialization;

internal static class IndomitableProgressionDetailDataMapper
{
    public static IndomitableProgressionDetail Map(
        IndomitableProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        IndomitableUseGrantData[] usesData =
            data.UsesByLevel
            ?? throw new ArgumentException(
                "Indomitable progression uses by level is required.",
                nameof(data));

        IndomitableUseGrant[] usesByLevel = usesData
            .Select(MapGrant)
            .ToArray();

        return new IndomitableProgressionDetail(
            usesByLevel,
            data.RecoversOnShortRest);
    }

    private static IndomitableUseGrant MapGrant(IndomitableUseGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new IndomitableUseGrant(data.CharacterLevel, data.UsesPerRest);
    }
}
