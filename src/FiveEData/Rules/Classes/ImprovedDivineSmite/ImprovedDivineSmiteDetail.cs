using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.ImprovedDivineSmite;

public sealed record ImprovedDivineSmiteDetail
{
    public ImprovedDivineSmiteDetail(
        DiceExpression damage,
        DamageTypeId damageTypeId,
        bool requiresMeleeWeapon)
    {
        if (string.IsNullOrWhiteSpace(damageTypeId.Value))
        {
            throw new ArgumentException(
                "Improved Divine Smite damage type ID is required.",
                nameof(damageTypeId));
        }

        Damage = damage;
        DamageTypeId = damageTypeId;
        RequiresMeleeWeapon = requiresMeleeWeapon;
    }

    public DiceExpression Damage { get; }

    public DamageTypeId DamageTypeId { get; }

    public bool RequiresMeleeWeapon { get; }
}
