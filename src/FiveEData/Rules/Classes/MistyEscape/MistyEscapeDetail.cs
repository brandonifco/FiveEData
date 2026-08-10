namespace FiveEData.Rules.Classes.MistyEscape;

public sealed record MistyEscapeDetail
{
    public MistyEscapeDetail(
        int teleportRangeFeet,
        bool grantsInvisibility,
        bool recoversOnShortRest)
    {
        if (teleportRangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(teleportRangeFeet),
                teleportRangeFeet,
                "Misty Escape teleport range must be greater than zero.");
        }

        TeleportRangeFeet = teleportRangeFeet;
        GrantsInvisibility = grantsInvisibility;
        RecoversOnShortRest = recoversOnShortRest;
    }

    public int TeleportRangeFeet { get; }

    public bool GrantsInvisibility { get; }

    public bool RecoversOnShortRest { get; }
}
