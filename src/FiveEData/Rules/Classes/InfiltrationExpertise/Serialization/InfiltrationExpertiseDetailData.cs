using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.InfiltrationExpertise.Serialization;

internal sealed class InfiltrationExpertiseDetailData
{
    [JsonRequired]
    public int RequiredDays { get; init; }

    [JsonRequired]
    public int CostGoldPieces { get; init; }
}
