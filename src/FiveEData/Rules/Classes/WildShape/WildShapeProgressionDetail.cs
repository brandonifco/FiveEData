namespace FiveEData.Rules.Classes.WildShape;

public sealed record WildShapeProgressionDetail
{
    public WildShapeProgressionDetail(
        IEnumerable<WildShapeFormLimit> formLimitsByLevel,
        int usesPerRest,
        bool recoversOnShortRest)
    {
        ArgumentNullException.ThrowIfNull(formLimitsByLevel);

        if (usesPerRest <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(usesPerRest),
                usesPerRest,
                "Uses per rest must be greater than zero.");
        }

        FormLimitsByLevel = Array.AsReadOnly(formLimitsByLevel.ToArray());
        UsesPerRest = usesPerRest;
        RecoversOnShortRest = recoversOnShortRest;
    }

    public IReadOnlyList<WildShapeFormLimit> FormLimitsByLevel { get; }
    public int UsesPerRest { get; }
    public bool RecoversOnShortRest { get; }
}
