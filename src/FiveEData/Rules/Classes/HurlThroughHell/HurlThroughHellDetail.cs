using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.HurlThroughHell;

public sealed record HurlThroughHellDetail
{
    public HurlThroughHellDetail(
        DiceExpression damage,
        DamageTypeId damageTypeId,
        bool exemptsFiends,
        bool recoversOnLongRest)
    {
        if (string.IsNullOrWhiteSpace(damageTypeId.Value))
        {
            throw new ArgumentException(
                "Hurl Through Hell damage type ID is required.",
                nameof(damageTypeId));
        }

        Damage = damage;
        DamageTypeId = damageTypeId;
        ExemptsFiends = exemptsFiends;
        RecoversOnLongRest = recoversOnLongRest;
    }

    public DiceExpression Damage { get; }

    public DamageTypeId DamageTypeId { get; }

    public bool ExemptsFiends { get; }

    public bool RecoversOnLongRest { get; }
}
