namespace FiveEData.Rules.Classes.AwakenedMind;

public sealed record AwakenedMindDetail
{
    public AwakenedMindDetail(int telepathyRangeFeet)
    {
        if (telepathyRangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(telepathyRangeFeet),
                telepathyRangeFeet,
                "Awakened Mind telepathy range must be greater than zero.");
        }

        TelepathyRangeFeet = telepathyRangeFeet;
    }

    public int TelepathyRangeFeet { get; }
}
