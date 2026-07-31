namespace FiveEData.Rules.Equipment.Armor;

public readonly record struct ArmorProficiencyConsequences
{
    public ArmorProficiencyConsequences(
        bool disadvantageOnStrengthOrDexterityAbilityChecks,
        bool disadvantageOnStrengthOrDexteritySavingThrows,
        bool disadvantageOnStrengthOrDexterityAttackRolls,
        bool preventsSpellcasting)
    {
        DisadvantageOnStrengthOrDexterityAbilityChecks =
            disadvantageOnStrengthOrDexterityAbilityChecks;
        DisadvantageOnStrengthOrDexteritySavingThrows =
            disadvantageOnStrengthOrDexteritySavingThrows;
        DisadvantageOnStrengthOrDexterityAttackRolls =
            disadvantageOnStrengthOrDexterityAttackRolls;
        PreventsSpellcasting = preventsSpellcasting;
    }

    public bool DisadvantageOnStrengthOrDexterityAbilityChecks { get; }
    public bool DisadvantageOnStrengthOrDexteritySavingThrows { get; }
    public bool DisadvantageOnStrengthOrDexterityAttackRolls { get; }
    public bool PreventsSpellcasting { get; }
}
