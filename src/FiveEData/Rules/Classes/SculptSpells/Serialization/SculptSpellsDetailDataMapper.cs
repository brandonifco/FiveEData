namespace FiveEData.Rules.Classes.SculptSpells.Serialization;

internal static class SculptSpellsDetailDataMapper
{
    public static SculptSpellsDetail Map(SculptSpellsDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new SculptSpellsDetail(
            data.ProtectsCreatureCountEqualToOnePlusSpellLevel,
            data.GrantsNoDamageOnSuccessfulSave);
    }
}
