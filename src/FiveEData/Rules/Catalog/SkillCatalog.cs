using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Catalog;

public sealed class SkillCatalog
{
    private readonly FrozenDictionary<
        SkillId,
        SkillDefinition> _byId;

    internal SkillCatalog(
        IEnumerable<SkillDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        SkillDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (SkillDefinition definition in ordered)
        {
            SkillDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<SkillDefinition> All { get; }
    public int Count => All.Count;

    public SkillDefinition Get(SkillId id)
    {
        if (_byId.TryGetValue(
                id,
                out SkillDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Skill '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        SkillId id,
        out SkillDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<SkillDefinition> definitions)
    {
        var ids = new HashSet<SkillId>();

        foreach (SkillDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate skill ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
