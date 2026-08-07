using System.Collections.Frozen;
using FiveEData.Rules.Spells.MagicSchools;

namespace FiveEData.Rules.Catalog;

public sealed class MagicSchoolCatalog
{
    private readonly FrozenDictionary<
        MagicSchoolId,
        MagicSchoolDefinition> _byId;

    internal MagicSchoolCatalog(
        IEnumerable<MagicSchoolDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        MagicSchoolDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (MagicSchoolDefinition definition in ordered)
        {
            MagicSchoolDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<MagicSchoolDefinition> All { get; }
    public int Count => All.Count;

    public MagicSchoolDefinition Get(MagicSchoolId id)
    {
        if (_byId.TryGetValue(
                id,
                out MagicSchoolDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Magic school '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        MagicSchoolId id,
        out MagicSchoolDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<MagicSchoolDefinition> definitions)
    {
        var ids = new HashSet<MagicSchoolId>();

        foreach (MagicSchoolDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate magic school ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
