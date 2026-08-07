namespace FiveEData.Rules.Creatures.Races.RelentlessEndurance;

public sealed record RelentlessEnduranceDetail
{
    public RelentlessEnduranceDetail(
        int hitPointsRetained,
        bool recoversOnLongRest)
    {
        if (hitPointsRetained <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hitPointsRetained),
                hitPointsRetained,
                "Relentless Endurance hit points retained must be greater " +
                "than zero.");
        }

        HitPointsRetained = hitPointsRetained;
        RecoversOnLongRest = recoversOnLongRest;
    }

    public int HitPointsRetained { get; }

    public bool RecoversOnLongRest { get; }
}
