namespace FiveEData.Rules.Classes.CircleForms;

public sealed record CircleFormsProgressionDetail
{
    public CircleFormsProgressionDetail(
        IEnumerable<CircleFormsChallengeRatingGrant> maxChallengeRatingByLevel)
    {
        ArgumentNullException.ThrowIfNull(maxChallengeRatingByLevel);

        MaxChallengeRatingByLevel =
            Array.AsReadOnly(maxChallengeRatingByLevel.ToArray());
    }

    public IReadOnlyList<CircleFormsChallengeRatingGrant> MaxChallengeRatingByLevel
    {
        get;
    }
}
