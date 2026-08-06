using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.CircleForms.Serialization;

internal sealed class CircleFormsProgressionDetailData
{
    [JsonRequired]
    public CircleFormsChallengeRatingGrantData[]? MaxChallengeRatingByLevel
    {
        get;
        init;
    }
}

internal sealed class CircleFormsChallengeRatingGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public double MaxChallengeRating { get; init; }
}
