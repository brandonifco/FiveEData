using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.RelentlessRage;

public sealed record RelentlessRageDetail
{
    public RelentlessRageDetail(
        AbilityId savingThrowAbilityId,
        int initialSavingThrowDC,
        int savingThrowDCIncreasePerUse,
        int hitPointsRetained,
        bool resetsOnShortRest)
    {
        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Relentless Rage saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        if (initialSavingThrowDC <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(initialSavingThrowDC),
                initialSavingThrowDC,
                "Relentless Rage initial saving throw DC must be greater " +
                "than zero.");
        }

        if (savingThrowDCIncreasePerUse <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(savingThrowDCIncreasePerUse),
                savingThrowDCIncreasePerUse,
                "Relentless Rage saving throw DC increase per use must be " +
                "greater than zero.");
        }

        if (hitPointsRetained <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hitPointsRetained),
                hitPointsRetained,
                "Relentless Rage hit points retained must be greater than " +
                "zero.");
        }

        SavingThrowAbilityId = savingThrowAbilityId;
        InitialSavingThrowDC = initialSavingThrowDC;
        SavingThrowDCIncreasePerUse = savingThrowDCIncreasePerUse;
        HitPointsRetained = hitPointsRetained;
        ResetsOnShortRest = resetsOnShortRest;
    }

    public AbilityId SavingThrowAbilityId { get; }

    public int InitialSavingThrowDC { get; }

    public int SavingThrowDCIncreasePerUse { get; }

    public int HitPointsRetained { get; }

    public bool ResetsOnShortRest { get; }
}
