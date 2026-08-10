using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.TransmutersStoneOptions;

public sealed class TransmutersStoneOptionDefinition
{
    internal TransmutersStoneOptionDefinition(
        TransmutersStoneOptionId id,
        string name,
        int? darkvisionRangeFeet,
        int? speedBonusFeet,
        bool requiresUnencumbered,
        AbilityId? savingThrowProficiencyAbilityId,
        IEnumerable<DamageTypeId> choosableResistedDamageTypeIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(choosableResistedDamageTypeIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        DarkvisionRangeFeet = darkvisionRangeFeet;
        SpeedBonusFeet = speedBonusFeet;
        RequiresUnencumbered = requiresUnencumbered;
        SavingThrowProficiencyAbilityId = savingThrowProficiencyAbilityId;
        ChoosableResistedDamageTypeIds =
            Array.AsReadOnly(choosableResistedDamageTypeIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public TransmutersStoneOptionId Id { get; }
    public string Name { get; }
    public int? DarkvisionRangeFeet { get; }
    public int? SpeedBonusFeet { get; }
    public bool RequiresUnencumbered { get; }
    public AbilityId? SavingThrowProficiencyAbilityId { get; }
    public IReadOnlyList<DamageTypeId> ChoosableResistedDamageTypeIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
