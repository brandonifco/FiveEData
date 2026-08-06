namespace FiveEData.Rules.Classes.Auras;

public readonly record struct AuraRange
{
    public AuraRange(int baseRangeFeet, int expandedRangeFeet, int expandedAtLevel)
    {
        if (baseRangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseRangeFeet),
                baseRangeFeet,
                "Base range must be greater than zero.");
        }

        if (expandedRangeFeet <= baseRangeFeet)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expandedRangeFeet),
                expandedRangeFeet,
                "Expanded range must be greater than the base range.");
        }

        if (expandedAtLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expandedAtLevel),
                expandedAtLevel,
                "Expanded-at level must be between 1 and 20.");
        }

        BaseRangeFeet = baseRangeFeet;
        ExpandedRangeFeet = expandedRangeFeet;
        ExpandedAtLevel = expandedAtLevel;
    }

    public int BaseRangeFeet { get; }

    public int ExpandedRangeFeet { get; }

    public int ExpandedAtLevel { get; }
}
