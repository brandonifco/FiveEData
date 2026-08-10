namespace FiveEData.Rules.Classes.PotentCantrip;

public sealed record PotentCantripDetail
{
    public PotentCantripDetail(
        bool grantsHalfDamageOnSuccessfulSave,
        bool negatesAdditionalCantripEffectsOnSuccessfulSave)
    {
        GrantsHalfDamageOnSuccessfulSave = grantsHalfDamageOnSuccessfulSave;
        NegatesAdditionalCantripEffectsOnSuccessfulSave =
            negatesAdditionalCantripEffectsOnSuccessfulSave;
    }

    public bool GrantsHalfDamageOnSuccessfulSave { get; }

    public bool NegatesAdditionalCantripEffectsOnSuccessfulSave { get; }
}
