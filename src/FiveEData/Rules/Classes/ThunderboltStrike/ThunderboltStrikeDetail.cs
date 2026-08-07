using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Classes.ThunderboltStrike;

public sealed record ThunderboltStrikeDetail
{
    public ThunderboltStrikeDetail(
        int pushDistanceFeet,
        CreatureSizeId maximumTargetSizeId)
    {
        if (pushDistanceFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pushDistanceFeet),
                pushDistanceFeet,
                "Thunderbolt Strike push distance must be greater than " +
                "zero.");
        }

        if (string.IsNullOrWhiteSpace(maximumTargetSizeId.Value))
        {
            throw new ArgumentException(
                "Thunderbolt Strike maximum target size ID is required.",
                nameof(maximumTargetSizeId));
        }

        PushDistanceFeet = pushDistanceFeet;
        MaximumTargetSizeId = maximumTargetSizeId;
    }

    public int PushDistanceFeet { get; }

    public CreatureSizeId MaximumTargetSizeId { get; }
}
