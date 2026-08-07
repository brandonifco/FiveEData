using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Classes.ThunderboltStrike.Serialization;

internal static class ThunderboltStrikeDetailDataMapper
{
    public static ThunderboltStrikeDetail Map(
        ThunderboltStrikeDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string maximumTargetSizeIdValue =
            data.MaximumTargetSizeId
            ?? throw new ArgumentException(
                "Thunderbolt Strike maximum target size ID is required.",
                nameof(data));

        return new ThunderboltStrikeDetail(
            data.PushDistanceFeet,
            new CreatureSizeId(maximumTargetSizeIdValue));
    }
}
