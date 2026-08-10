using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.AlterMemories.Serialization;

internal static class AlterMemoriesDetailDataMapper
{
    public static AlterMemoriesDetail Map(AlterMemoriesDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string forgetSavingThrowAbilityIdValue =
            data.ForgetSavingThrowAbilityId
            ?? throw new ArgumentException(
                "Alter Memories forget saving throw ability ID is required.",
                nameof(data));

        return new AlterMemoriesDetail(
            data.MakesCreatureUnawareOfCharm,
            new AbilityId(forgetSavingThrowAbilityIdValue));
    }
}
