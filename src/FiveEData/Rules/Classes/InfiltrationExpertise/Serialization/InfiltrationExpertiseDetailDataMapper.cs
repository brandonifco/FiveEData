namespace FiveEData.Rules.Classes.InfiltrationExpertise.Serialization;

internal static class InfiltrationExpertiseDetailDataMapper
{
    public static InfiltrationExpertiseDetail Map(
        InfiltrationExpertiseDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new InfiltrationExpertiseDetail(
            data.RequiredDays,
            data.CostGoldPieces);
    }
}
