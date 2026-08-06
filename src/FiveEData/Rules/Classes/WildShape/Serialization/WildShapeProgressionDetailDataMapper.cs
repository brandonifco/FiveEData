namespace FiveEData.Rules.Classes.WildShape.Serialization;

internal static class WildShapeProgressionDetailDataMapper
{
    public static WildShapeProgressionDetail Map(
        WildShapeProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        WildShapeFormLimitData[] formLimitsData =
            data.FormLimitsByLevel
            ?? throw new ArgumentException(
                "Wild Shape progression form limits by level are required.",
                nameof(data));

        WildShapeFormLimit[] formLimitsByLevel = formLimitsData
            .Select(
                limit => new WildShapeFormLimit(
                    limit.CharacterLevel,
                    limit.MaxChallengeRating,
                    limit.AllowsFlyingSpeed,
                    limit.AllowsSwimmingSpeed))
            .ToArray();

        return new WildShapeProgressionDetail(
            formLimitsByLevel,
            data.UsesPerRest,
            data.RecoversOnShortRest);
    }
}
