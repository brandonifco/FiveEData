namespace FiveEData.Rules.Classes.CircleForms.Serialization;

internal static class CircleFormsProgressionDetailDataMapper
{
    public static CircleFormsProgressionDetail Map(
        CircleFormsProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        CircleFormsChallengeRatingGrantData[] grantsData =
            data.MaxChallengeRatingByLevel
            ?? throw new ArgumentException(
                "Circle Forms progression max challenge rating by level " +
                "is required.",
                nameof(data));

        CircleFormsChallengeRatingGrant[] maxChallengeRatingByLevel =
            grantsData
                .Select(
                    grant => new CircleFormsChallengeRatingGrant(
                        grant.CharacterLevel,
                        grant.MaxChallengeRating))
                .ToArray();

        return new CircleFormsProgressionDetail(maxChallengeRatingByLevel);
    }
}
