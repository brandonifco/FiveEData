namespace FiveEData.Rules.Classes.ActionSurge.Serialization;

internal static class ActionSurgeProgressionDetailDataMapper
{
    public static ActionSurgeProgressionDetail Map(
        ActionSurgeProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ActionSurgeUseGrantData[] usesData =
            data.UsesByLevel
            ?? throw new ArgumentException(
                "Action Surge progression uses by level is required.",
                nameof(data));

        ActionSurgeUseGrant[] usesByLevel = usesData
            .Select(MapGrant)
            .ToArray();

        return new ActionSurgeProgressionDetail(
            usesByLevel,
            data.RecoversOnShortRest,
            data.OncePerTurn);
    }

    private static ActionSurgeUseGrant MapGrant(ActionSurgeUseGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new ActionSurgeUseGrant(data.CharacterLevel, data.UsesPerRest);
    }
}
