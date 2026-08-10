namespace FiveEData.Rules.Classes.ElementalAffinity;

public sealed record ElementalAffinityDetail
{
    public ElementalAffinityDetail(
        bool addsSpellcastingModifierToDamage,
        int resistanceSorceryPointCost,
        int resistanceDurationHours)
    {
        if (resistanceSorceryPointCost <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(resistanceSorceryPointCost),
                resistanceSorceryPointCost,
                "Elemental Affinity resistance sorcery point cost must be " +
                "greater than zero.");
        }

        if (resistanceDurationHours <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(resistanceDurationHours),
                resistanceDurationHours,
                "Elemental Affinity resistance duration must be greater " +
                "than zero.");
        }

        AddsSpellcastingModifierToDamage = addsSpellcastingModifierToDamage;
        ResistanceSorceryPointCost = resistanceSorceryPointCost;
        ResistanceDurationHours = resistanceDurationHours;
    }

    public bool AddsSpellcastingModifierToDamage { get; }

    public int ResistanceSorceryPointCost { get; }

    public int ResistanceDurationHours { get; }
}
