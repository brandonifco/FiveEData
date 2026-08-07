namespace FiveEData.Rules.Creatures.Races.SavageAttacks.Serialization;

internal static class SavageAttacksDetailDataMapper
{
    public static SavageAttacksDetail Map(SavageAttacksDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new SavageAttacksDetail(
            data.AdditionalCriticalDice,
            data.RequiresMeleeWeapon);
    }
}
