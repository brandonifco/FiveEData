using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.Overchannel;

public sealed record OverchannelDetail
{
    public OverchannelDetail(
        int maximumSpellLevel,
        bool dealsMaximumDamage,
        bool firstUseHasNoAdverseEffect,
        DiceExpression selfDamagePerSpellLevel,
        DamageTypeId selfDamageTypeId,
        DiceExpression selfDamageIncreasePerSubsequentUse,
        bool ignoresResistanceAndImmunity,
        bool recoversOnLongRest)
    {
        if (maximumSpellLevel <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumSpellLevel),
                maximumSpellLevel,
                "Overchannel maximum spell level must be greater than " +
                "zero.");
        }

        if (string.IsNullOrWhiteSpace(selfDamageTypeId.Value))
        {
            throw new ArgumentException(
                "Overchannel self damage type ID is required.",
                nameof(selfDamageTypeId));
        }

        MaximumSpellLevel = maximumSpellLevel;
        DealsMaximumDamage = dealsMaximumDamage;
        FirstUseHasNoAdverseEffect = firstUseHasNoAdverseEffect;
        SelfDamagePerSpellLevel = selfDamagePerSpellLevel;
        SelfDamageTypeId = selfDamageTypeId;
        SelfDamageIncreasePerSubsequentUse =
            selfDamageIncreasePerSubsequentUse;
        IgnoresResistanceAndImmunity = ignoresResistanceAndImmunity;
        RecoversOnLongRest = recoversOnLongRest;
    }

    public int MaximumSpellLevel { get; }

    public bool DealsMaximumDamage { get; }

    public bool FirstUseHasNoAdverseEffect { get; }

    public DiceExpression SelfDamagePerSpellLevel { get; }

    public DamageTypeId SelfDamageTypeId { get; }

    public DiceExpression SelfDamageIncreasePerSubsequentUse { get; }

    public bool IgnoresResistanceAndImmunity { get; }

    public bool RecoversOnLongRest { get; }
}
