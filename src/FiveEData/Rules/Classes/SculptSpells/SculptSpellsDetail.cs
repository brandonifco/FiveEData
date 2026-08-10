namespace FiveEData.Rules.Classes.SculptSpells;

public sealed record SculptSpellsDetail
{
    public SculptSpellsDetail(
        bool protectsCreatureCountEqualToOnePlusSpellLevel,
        bool grantsNoDamageOnSuccessfulSave)
    {
        ProtectsCreatureCountEqualToOnePlusSpellLevel =
            protectsCreatureCountEqualToOnePlusSpellLevel;
        GrantsNoDamageOnSuccessfulSave = grantsNoDamageOnSuccessfulSave;
    }

    public bool ProtectsCreatureCountEqualToOnePlusSpellLevel { get; }

    public bool GrantsNoDamageOnSuccessfulSave { get; }
}
