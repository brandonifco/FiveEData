namespace FiveEData.Rules.Classes.WizardSpellbook.Serialization;

internal static class WizardSpellbookDetailDataMapper
{
    public static WizardSpellbookDetail Map(WizardSpellbookDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new WizardSpellbookDetail(
            data.StartingSpellCount,
            data.SpellsAddedPerLevelAfterFirst);
    }
}
