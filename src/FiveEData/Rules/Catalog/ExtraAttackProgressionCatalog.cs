using System.Collections.Frozen;
using FiveEData.Rules.Classes.ExtraAttack;

namespace FiveEData.Rules.Catalog;

public sealed class ExtraAttackProgressionCatalog
{
    private readonly FrozenDictionary<
        ExtraAttackProgressionId,
        ExtraAttackProgressionDefinition> _byId;

    internal ExtraAttackProgressionCatalog(
        IEnumerable<ExtraAttackProgressionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ExtraAttackProgressionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ExtraAttackProgressionDefinition definition in ordered)
        {
            ExtraAttackProgressionDefinitionValidator.EnsureValid(
                definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ExtraAttackProgressionDefinition> All { get; }
    public int Count => All.Count;

    public ExtraAttackProgressionDefinition Get(
        ExtraAttackProgressionId id)
    {
        if (_byId.TryGetValue(
                id,
                out ExtraAttackProgressionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Extra Attack progression '{id}' does not exist in " +
            "this catalog.");
    }

    public bool TryGet(
        ExtraAttackProgressionId id,
        out ExtraAttackProgressionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ExtraAttackProgressionDefinition> definitions)
    {
        var ids = new HashSet<ExtraAttackProgressionId>();

        foreach (ExtraAttackProgressionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    "Duplicate Extra Attack progression ID " +
                    $"'{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
