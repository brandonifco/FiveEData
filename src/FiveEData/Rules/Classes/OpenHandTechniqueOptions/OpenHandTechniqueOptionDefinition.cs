using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.OpenHandTechniqueOptions;

public sealed class OpenHandTechniqueOptionDefinition
{
    internal OpenHandTechniqueOptionDefinition(
        OpenHandTechniqueOptionId id,
        string name,
        AbilityId? savingThrowAbilityId,
        ConditionId? imposedConditionId,
        int? pushDistanceFeet,
        bool preventsReactions,
        NextTurnDurationTrigger? preventsReactionsUntil,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        SavingThrowAbilityId = savingThrowAbilityId;
        ImposedConditionId = imposedConditionId;
        PushDistanceFeet = pushDistanceFeet;
        PreventsReactions = preventsReactions;
        PreventsReactionsUntil = preventsReactionsUntil;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public OpenHandTechniqueOptionId Id { get; }
    public string Name { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public ConditionId? ImposedConditionId { get; }
    public int? PushDistanceFeet { get; }
    public bool PreventsReactions { get; }
    public NextTurnDurationTrigger? PreventsReactionsUntil { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
