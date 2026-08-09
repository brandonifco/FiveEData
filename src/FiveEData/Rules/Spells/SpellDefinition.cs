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
        bool isRitual,
        SpellDamageEffect? damageEffect,
        SpellConditionEffect? conditionEffect,
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
        IsRitual = isRitual;
        DamageEffect = damageEffect;
        ConditionEffect = conditionEffect;
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

    /// <summary>
    /// True when the spell carries the PHB's ritual tag (p.201), which
    /// lets it be cast without expending a slot at ten minutes' extra
    /// casting time. No cantrip is a ritual.
    /// </summary>
    public bool IsRitual { get; }

    /// <summary>
    /// The spell's own damage-resolution mechanic, when it deals damage
    /// directly (an attack roll or a saving throw, plus the damage dice).
    /// Null for a spell with no damage effect of its own — most utility and
    /// buff cantrips.
    /// </summary>
    public SpellDamageEffect? DamageEffect { get; }

    /// <summary>
    /// The named Appendix A condition(s) the spell imposes on a failed
    /// saving throw, independent of any <see cref="DamageEffect"/> — Ray
    /// of Sickness carries both. Null for a spell with no condition effect
    /// of its own.
    /// </summary>
    public SpellConditionEffect? ConditionEffect { get; }

    public IReadOnlyList<ClassId> AvailableToClassIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }

    public bool IsCantrip => Level == 0;
}
