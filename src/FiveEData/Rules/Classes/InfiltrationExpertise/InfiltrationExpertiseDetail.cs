namespace FiveEData.Rules.Classes.InfiltrationExpertise;

public sealed record InfiltrationExpertiseDetail
{
    public InfiltrationExpertiseDetail(
        int requiredDays,
        int costGoldPieces)
    {
        if (requiredDays <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredDays),
                requiredDays,
                "Infiltration Expertise required days must be greater than " +
                "zero.");
        }

        if (costGoldPieces <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(costGoldPieces),
                costGoldPieces,
                "Infiltration Expertise cost must be greater than zero.");
        }

        RequiredDays = requiredDays;
        CostGoldPieces = costGoldPieces;
    }

    public int RequiredDays { get; }

    public int CostGoldPieces { get; }
}
