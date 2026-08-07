namespace FiveEData.Rules.Classes.Portent.Serialization;

internal static class PortentProgressionDetailDataMapper
{
    public static PortentProgressionDetail Map(
        PortentProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        PortentRollGrantData[] grantData =
            data.ForetellingRollsByLevel
            ?? throw new ArgumentException(
                "Portent progression foretelling rolls by level is required.",
                nameof(data));

        PortentRollGrant[] foretellingRollsByLevel = grantData
            .Select(MapGrant)
            .ToArray();

        return new PortentProgressionDetail(
            foretellingRollsByLevel,
            data.OncePerTurn,
            data.RecoversOnLongRest);
    }

    private static PortentRollGrant MapGrant(PortentRollGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new PortentRollGrant(
            data.CharacterLevel,
            data.ForetellingRolls);
    }
}
