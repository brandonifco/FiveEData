using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.RelentlessRage.Serialization;

internal static class RelentlessRageDetailDataMapper
{
    public static RelentlessRageDetail Map(RelentlessRageDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Relentless Rage saving throw ability ID is required.",
                nameof(data));

        return new RelentlessRageDetail(
            new AbilityId(savingThrowAbilityIdValue),
            data.InitialSavingThrowDC,
            data.SavingThrowDCIncreasePerUse,
            data.HitPointsRetained,
            data.ResetsOnShortRest);
    }
}
