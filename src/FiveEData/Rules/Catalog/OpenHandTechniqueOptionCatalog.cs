using System.Collections.Frozen;
using FiveEData.Rules.Classes.OpenHandTechniqueOptions;

namespace FiveEData.Rules.Catalog;

public sealed class OpenHandTechniqueOptionCatalog
{
    private readonly FrozenDictionary<
        OpenHandTechniqueOptionId,
        OpenHandTechniqueOptionDefinition> _byId;

    internal OpenHandTechniqueOptionCatalog(
        IEnumerable<OpenHandTechniqueOptionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        OpenHandTechniqueOptionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (OpenHandTechniqueOptionDefinition definition in ordered)
        {
            OpenHandTechniqueOptionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<OpenHandTechniqueOptionDefinition> All { get; }
    public int Count => All.Count;

    public OpenHandTechniqueOptionDefinition Get(OpenHandTechniqueOptionId id)
    {
        if (_byId.TryGetValue(id, out OpenHandTechniqueOptionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Open hand technique option '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        OpenHandTechniqueOptionId id,
        out OpenHandTechniqueOptionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<OpenHandTechniqueOptionDefinition> definitions)
    {
        var ids = new HashSet<OpenHandTechniqueOptionId>();

        foreach (OpenHandTechniqueOptionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate open hand technique option ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
