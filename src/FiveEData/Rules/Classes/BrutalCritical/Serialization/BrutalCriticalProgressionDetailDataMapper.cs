namespace FiveEData.Rules.Classes.BrutalCritical.Serialization;

internal static class BrutalCriticalProgressionDetailDataMapper
{
    public static BrutalCriticalProgressionDetail Map(
        BrutalCriticalProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        BrutalCriticalDiceGrantData[] diceData =
            data.AdditionalDiceByLevel
            ?? throw new ArgumentException(
                "Brutal Critical progression additional dice by level is " +
                "required.",
                nameof(data));

        BrutalCriticalDiceGrant[] additionalDiceByLevel = diceData
            .Select(MapGrant)
            .ToArray();

        return new BrutalCriticalProgressionDetail(
            additionalDiceByLevel,
            data.RequiresMeleeAttack);
    }

    private static BrutalCriticalDiceGrant MapGrant(
        BrutalCriticalDiceGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new BrutalCriticalDiceGrant(
            data.CharacterLevel,
            data.AdditionalDice);
    }
}
