using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.SculptSpells.Serialization;

internal sealed class SculptSpellsDetailData
{
    [JsonRequired]
    public bool ProtectsCreatureCountEqualToOnePlusSpellLevel { get; init; }

    [JsonRequired]
    public bool GrantsNoDamageOnSuccessfulSave { get; init; }
}
