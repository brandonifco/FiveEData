using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.Spellcasting;

public sealed class SpellSlotProgressionDefinition
{
    internal SpellSlotProgressionDefinition(
        SpellSlotProgressionId id,
        string name,
        SpellSlotRecoveryRest recoveryRest,
        IEnumerable<SpellSlotLevelEntry> levelEntries,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(levelEntries);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RecoveryRest = recoveryRest;
        LevelEntries = Array.AsReadOnly(levelEntries.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SpellSlotProgressionId Id { get; }
    public string Name { get; }
    public SpellSlotRecoveryRest RecoveryRest { get; }
    public IReadOnlyList<SpellSlotLevelEntry> LevelEntries { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
