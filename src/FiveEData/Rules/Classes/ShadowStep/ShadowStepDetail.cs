namespace FiveEData.Rules.Classes.ShadowStep;

public sealed record ShadowStepDetail
{
    public ShadowStepDetail(
        int teleportRangeFeet,
        bool grantsAdvantageOnNextMeleeAttack)
    {
        if (teleportRangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(teleportRangeFeet),
                teleportRangeFeet,
                "Shadow Step teleport range must be greater than zero.");
        }

        TeleportRangeFeet = teleportRangeFeet;
        GrantsAdvantageOnNextMeleeAttack = grantsAdvantageOnNextMeleeAttack;
    }

    public int TeleportRangeFeet { get; }

    public bool GrantsAdvantageOnNextMeleeAttack { get; }
}
