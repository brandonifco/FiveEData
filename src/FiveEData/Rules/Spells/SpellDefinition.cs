using FiveEData.Rules.Classes;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Spells.MagicSchools;

namespace FiveEData.Rules.Spells;

public sealed class SpellDefinition
{
    internal SpellDefinition(
        SpellId id,
        string name,
        int level,
        MagicSchoolId schoolId,
        SpellCastingTime castingTime,
        SpellRange range,
        SpellComponents components,
        SpellDuration duration,
        IEnumerable<ClassId> availableToClassIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(availableToClassIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Level = level;
        SchoolId = schoolId;
        CastingTime = castingTime;
        Range = range;
        Components = components;
        Duration = duration;
        AvailableToClassIds =
            Array.AsReadOnly(availableToClassIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SpellId Id { get; }
    public string Name { get; }

    /// <summary>Spell level, 0 through 9. A cantrip is level 0.</summary>
    public int Level { get; }

    public MagicSchoolId SchoolId { get; }
    public SpellCastingTime CastingTime { get; }
    public SpellRange Range { get; }
    public SpellComponents Components { get; }
    public SpellDuration Duration { get; }
    public IReadOnlyList<ClassId> AvailableToClassIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }

    public bool IsCantrip => Level == 0;
}
