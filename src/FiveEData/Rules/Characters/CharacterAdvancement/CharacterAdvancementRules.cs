using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Characters.CharacterAdvancement;

/// <summary>
/// The Character Advancement table (p.15) — the one progression that is
/// universal across every class rather than owned by one of them.
/// </summary>
public sealed class CharacterAdvancementRules
{
    public const int MaximumLevel = 20;

    private readonly IReadOnlyDictionary<int, CharacterAdvancementLevel>
        _byLevel;

    internal CharacterAdvancementRules(
        IEnumerable<CharacterAdvancementLevel> levels,
        IEnumerable<SourceReference> sources)
    {
        ArgumentNullException.ThrowIfNull(levels);
        ArgumentNullException.ThrowIfNull(sources);

        CharacterAdvancementLevel[] ordered = levels
            .OrderBy(level => level.Level)
            .ToArray();

        Levels = Array.AsReadOnly(ordered);
        Sources = Array.AsReadOnly(sources.ToArray());

        // Grouped rather than keyed directly so a duplicated level is
        // reported by the validator's contiguity check instead of throwing
        // an opaque dictionary error out of the constructor.
        _byLevel = ordered
            .GroupBy(level => level.Level)
            .ToDictionary(group => group.Key, group => group.First());
    }

    public IReadOnlyList<CharacterAdvancementLevel> Levels { get; }

    public IReadOnlyList<SourceReference> Sources { get; }

    public CharacterAdvancementLevel Get(int level)
    {
        if (_byLevel.TryGetValue(level, out CharacterAdvancementLevel found))
        {
            return found;
        }

        throw new KeyNotFoundException(
            $"Character level {level} is not in the advancement table.");
    }

    public bool TryGet(int level, out CharacterAdvancementLevel found)
    {
        return _byLevel.TryGetValue(level, out found);
    }

    /// <summary>
    /// The character level reached at the given experience point total.
    /// </summary>
    public int LevelForExperiencePoints(int experiencePoints)
    {
        if (experiencePoints < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(experiencePoints),
                experiencePoints,
                "Experience points cannot be negative.");
        }

        int level = Levels[0].Level;

        foreach (CharacterAdvancementLevel entry in Levels)
        {
            if (entry.ExperiencePointThreshold > experiencePoints)
            {
                break;
            }

            level = entry.Level;
        }

        return level;
    }
}
