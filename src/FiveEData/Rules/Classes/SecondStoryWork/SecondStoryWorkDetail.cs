namespace FiveEData.Rules.Classes.SecondStoryWork;

public sealed record SecondStoryWorkDetail
{
    public SecondStoryWorkDetail(
        bool climbingCostsNoExtraMovement,
        bool addsDexterityModifierToRunningJumpDistance)
    {
        ClimbingCostsNoExtraMovement = climbingCostsNoExtraMovement;
        AddsDexterityModifierToRunningJumpDistance =
            addsDexterityModifierToRunningJumpDistance;
    }

    public bool ClimbingCostsNoExtraMovement { get; }

    public bool AddsDexterityModifierToRunningJumpDistance { get; }
}
