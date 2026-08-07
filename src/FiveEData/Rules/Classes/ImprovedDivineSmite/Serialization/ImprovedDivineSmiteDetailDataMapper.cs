using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.ImprovedDivineSmite.Serialization;

internal static class ImprovedDivineSmiteDetailDataMapper
{
    public static ImprovedDivineSmiteDetail Map(
        ImprovedDivineSmiteDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData damageData =
            data.Damage
            ?? throw new ArgumentException(
                "Improved Divine Smite damage is required.",
                nameof(data));

        string damageTypeIdValue =
            data.DamageTypeId
            ?? throw new ArgumentException(
                "Improved Divine Smite damage type ID is required.",
                nameof(data));

        return new ImprovedDivineSmiteDetail(
            new DiceExpression(damageData.Count, damageData.Sides),
            new DamageTypeId(damageTypeIdValue),
            data.RequiresMeleeWeapon);
    }
}
