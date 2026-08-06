using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.BattleMasterManeuvers;

public sealed class BattleMasterManeuverDefinition
{
    internal BattleMasterManeuverDefinition(
        BattleMasterManeuverId id,
        string name,
        BattleMasterManeuverEffectTarget effectTarget,
        AbilityId? savingThrowAbilityId,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        EffectTarget = effectTarget;
        SavingThrowAbilityId = savingThrowAbilityId;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public BattleMasterManeuverId Id { get; }
    public string Name { get; }
    public BattleMasterManeuverEffectTarget EffectTarget { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
