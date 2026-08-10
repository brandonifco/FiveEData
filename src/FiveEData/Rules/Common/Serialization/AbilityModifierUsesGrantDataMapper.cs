using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Common.Serialization;

internal static class AbilityModifierUsesGrantDataMapper
{
    public static AbilityModifierUsesGrant Map(
        AbilityModifierUsesGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string abilityIdValue = data.AbilityId
            ?? throw new ArgumentException(
                "Ability modifier uses grant ability ID is required.",
                nameof(data));

        return new AbilityModifierUsesGrant(
            new AbilityId(abilityIdValue),
            data.RecoversOnLongRest);
    }
}
