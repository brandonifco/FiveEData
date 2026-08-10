using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.InstinctiveCharm.Serialization;

internal static class InstinctiveCharmDetailDataMapper
{
    public static InstinctiveCharmDetail Map(InstinctiveCharmDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Instinctive Charm saving throw ability ID is required.",
                nameof(data));

        return new InstinctiveCharmDetail(
            data.RangeFeet,
            new AbilityId(savingThrowAbilityIdValue),
            data.RedirectsAttackToClosestOtherCreature);
    }
}
