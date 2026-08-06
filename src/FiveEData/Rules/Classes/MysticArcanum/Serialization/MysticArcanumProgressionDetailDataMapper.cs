namespace FiveEData.Rules.Classes.MysticArcanum.Serialization;

internal static class MysticArcanumProgressionDetailDataMapper
{
    public static MysticArcanumProgressionDetail Map(
        MysticArcanumProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        MysticArcanumGrantData[] arcanumData =
            data.ArcanumByLevel
            ?? throw new ArgumentException(
                "Mystic Arcanum progression arcanum by level are required.",
                nameof(data));

        MysticArcanumGrant[] arcanumByLevel = arcanumData
            .Select(
                grant => new MysticArcanumGrant(
                    grant.CharacterLevel,
                    grant.SpellLevel))
            .ToArray();

        return new MysticArcanumProgressionDetail(
            arcanumByLevel,
            data.RecoversOnShortRest);
    }
}
