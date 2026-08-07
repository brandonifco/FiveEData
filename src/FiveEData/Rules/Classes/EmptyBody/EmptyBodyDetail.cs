namespace FiveEData.Rules.Classes.EmptyBody;

public sealed record EmptyBodyDetail
{
    public EmptyBodyDetail(
        int invisibilityKiCost,
        int invisibilityDurationMinutes,
        int astralProjectionKiCost)
    {
        if (invisibilityKiCost <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(invisibilityKiCost),
                invisibilityKiCost,
                "Empty Body invisibility ki cost must be greater than zero.");
        }

        if (invisibilityDurationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(invisibilityDurationMinutes),
                invisibilityDurationMinutes,
                "Empty Body invisibility duration must be greater than " +
                "zero.");
        }

        if (astralProjectionKiCost <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(astralProjectionKiCost),
                astralProjectionKiCost,
                "Empty Body astral projection ki cost must be greater than " +
                "zero.");
        }

        InvisibilityKiCost = invisibilityKiCost;
        InvisibilityDurationMinutes = invisibilityDurationMinutes;
        AstralProjectionKiCost = astralProjectionKiCost;
    }

    public int InvisibilityKiCost { get; }

    public int InvisibilityDurationMinutes { get; }

    public int AstralProjectionKiCost { get; }
}
