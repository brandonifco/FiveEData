using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.ThoughtShield.Serialization;

internal static class ThoughtShieldDetailDataMapper
{
    public static ThoughtShieldDetail Map(ThoughtShieldDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string resistedDamageTypeIdValue = data.ResistedDamageTypeId
            ?? throw new ArgumentException(
                "Thought Shield resisted damage type ID is required.",
                nameof(data));

        return new ThoughtShieldDetail(
            data.BlocksTelepathicReading,
            new DamageTypeId(resistedDamageTypeIdValue),
            data.ReflectsDamageToAttacker);
    }
}
