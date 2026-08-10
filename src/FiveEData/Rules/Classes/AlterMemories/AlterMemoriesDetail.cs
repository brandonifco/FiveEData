using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.AlterMemories;

public sealed record AlterMemoriesDetail
{
    public AlterMemoriesDetail(
        bool makesCreatureUnawareOfCharm,
        AbilityId forgetSavingThrowAbilityId)
    {
        if (string.IsNullOrWhiteSpace(forgetSavingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Alter Memories forget saving throw ability ID is required.",
                nameof(forgetSavingThrowAbilityId));
        }

        MakesCreatureUnawareOfCharm = makesCreatureUnawareOfCharm;
        ForgetSavingThrowAbilityId = forgetSavingThrowAbilityId;
    }

    public bool MakesCreatureUnawareOfCharm { get; }

    public AbilityId ForgetSavingThrowAbilityId { get; }
}
