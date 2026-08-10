using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Assassinate.Serialization;

internal sealed class AssassinateDetailData
{
    [JsonRequired]
    public bool GrantsAdvantageAgainstCreaturesThatHaveNotActed
    {
        get;
        init;
    }

    [JsonRequired]
    public bool HitsAgainstSurprisedCreaturesAreCritical { get; init; }
}
