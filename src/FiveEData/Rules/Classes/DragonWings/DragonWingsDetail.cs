namespace FiveEData.Rules.Classes.DragonWings;

public sealed record DragonWingsDetail
{
    public DragonWingsDetail(
        bool grantsFlyingSpeedEqualToCurrentSpeed,
        bool requiresBonusActionToCreateOrDismiss)
    {
        GrantsFlyingSpeedEqualToCurrentSpeed =
            grantsFlyingSpeedEqualToCurrentSpeed;
        RequiresBonusActionToCreateOrDismiss =
            requiresBonusActionToCreateOrDismiss;
    }

    public bool GrantsFlyingSpeedEqualToCurrentSpeed { get; }

    public bool RequiresBonusActionToCreateOrDismiss { get; }
}
