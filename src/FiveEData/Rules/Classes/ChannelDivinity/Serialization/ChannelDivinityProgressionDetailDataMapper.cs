namespace FiveEData.Rules.Classes.ChannelDivinity.Serialization;

internal static class ChannelDivinityProgressionDetailDataMapper
{
    public static ChannelDivinityProgressionDetail Map(
        ChannelDivinityProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ChannelDivinityUseGrantData[] usesData =
            data.UsesByLevel
            ?? throw new ArgumentException(
                "Channel Divinity progression uses by level are required.",
                nameof(data));

        ChannelDivinityUseGrant[] usesByLevel = usesData
            .Select(
                grant => new ChannelDivinityUseGrant(
                    grant.CharacterLevel,
                    grant.UsesPerRest))
            .ToArray();

        return new ChannelDivinityProgressionDetail(
            usesByLevel,
            data.RecoversOnShortRest);
    }
}
