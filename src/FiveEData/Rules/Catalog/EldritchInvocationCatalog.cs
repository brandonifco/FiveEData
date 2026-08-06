using System.Collections.Frozen;
using FiveEData.Rules.Classes.EldritchInvocations;

namespace FiveEData.Rules.Catalog;

public sealed class EldritchInvocationCatalog
{
    private readonly FrozenDictionary<
        EldritchInvocationId,
        EldritchInvocationDefinition> _byId;

    internal EldritchInvocationCatalog(
        IEnumerable<EldritchInvocationDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        EldritchInvocationDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (EldritchInvocationDefinition definition in ordered)
        {
            EldritchInvocationDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<EldritchInvocationDefinition> All { get; }
    public int Count => All.Count;

    public EldritchInvocationDefinition Get(EldritchInvocationId id)
    {
        if (_byId.TryGetValue(
                id,
                out EldritchInvocationDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Eldritch invocation '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        EldritchInvocationId id,
        out EldritchInvocationDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<EldritchInvocationDefinition> definitions)
    {
        var ids = new HashSet<EldritchInvocationId>();

        foreach (EldritchInvocationDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate eldritch invocation ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
