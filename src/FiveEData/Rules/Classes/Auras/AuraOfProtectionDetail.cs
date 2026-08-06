namespace FiveEData.Rules.Classes.Auras;

public sealed record AuraOfProtectionDetail
{
    public AuraOfProtectionDetail(
        AuraRange range,
        bool requiresConsciousness,
        int savingThrowBonusMinimum)
    {
        if (savingThrowBonusMinimum <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(savingThrowBonusMinimum),
                savingThrowBonusMinimum,
                "Saving throw bonus minimum must be greater than zero.");
        }

        Range = range;
        RequiresConsciousness = requiresConsciousness;
        SavingThrowBonusMinimum = savingThrowBonusMinimum;
    }

    public AuraRange Range { get; }
    public bool RequiresConsciousness { get; }
    public int SavingThrowBonusMinimum { get; }
}
