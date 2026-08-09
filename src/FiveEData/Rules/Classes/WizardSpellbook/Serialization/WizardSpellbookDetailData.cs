using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.WizardSpellbook.Serialization;

internal sealed class WizardSpellbookDetailData
{
    [JsonRequired]
    public int StartingSpellCount { get; init; }

    [JsonRequired]
    public int SpellsAddedPerLevelAfterFirst { get; init; }
}
