using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.ThoughtShield;

public sealed record ThoughtShieldDetail
{
    public ThoughtShieldDetail(
        bool blocksTelepathicReading,
        DamageTypeId resistedDamageTypeId,
        bool reflectsDamageToAttacker)
    {
        if (string.IsNullOrWhiteSpace(resistedDamageTypeId.Value))
        {
            throw new ArgumentException(
                "Thought Shield resisted damage type ID is required.",
                nameof(resistedDamageTypeId));
        }

        BlocksTelepathicReading = blocksTelepathicReading;
        ResistedDamageTypeId = resistedDamageTypeId;
        ReflectsDamageToAttacker = reflectsDamageToAttacker;
    }

    public bool BlocksTelepathicReading { get; }

    public DamageTypeId ResistedDamageTypeId { get; }

    public bool ReflectsDamageToAttacker { get; }
}
