using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.DraconicPresence;

public sealed record DraconicPresenceDetail
{
    public DraconicPresenceDetail(
        int sorceryPointCost,
        int rangeFeet,
        IEnumerable<ConditionId> choosableConditionIds,
        AbilityId savingThrowAbilityId,
        int durationMinutes,
        bool requiresConcentration)
    {
        if (sorceryPointCost <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sorceryPointCost),
                sorceryPointCost,
                "Draconic Presence sorcery point cost must be greater " +
                "than zero.");
        }

        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Draconic Presence range must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(choosableConditionIds);

        ConditionId[] choosableConditionIdArray =
            choosableConditionIds.ToArray();

        if (choosableConditionIdArray.Length < 2)
        {
            throw new ArgumentException(
                "Draconic Presence choosable condition IDs must contain " +
                "at least two options.",
                nameof(choosableConditionIds));
        }

        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Draconic Presence saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        if (durationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationMinutes),
                durationMinutes,
                "Draconic Presence duration must be greater than zero.");
        }

        SorceryPointCost = sorceryPointCost;
        RangeFeet = rangeFeet;
        ChoosableConditionIds = Array.AsReadOnly(choosableConditionIdArray);
        SavingThrowAbilityId = savingThrowAbilityId;
        DurationMinutes = durationMinutes;
        RequiresConcentration = requiresConcentration;
    }

    public int SorceryPointCost { get; }

    public int RangeFeet { get; }

    public IReadOnlyList<ConditionId> ChoosableConditionIds { get; }

    public AbilityId SavingThrowAbilityId { get; }

    public int DurationMinutes { get; }

    public bool RequiresConcentration { get; }
}
