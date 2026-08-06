using System.Collections.Frozen;
using FiveEData.Rules.Classes.ChannelDivinityOptions;

namespace FiveEData.Rules.Catalog;

public sealed class ChannelDivinityOptionCatalog
{
    private readonly FrozenDictionary<
        ChannelDivinityOptionId,
        ChannelDivinityOptionDefinition> _byId;

    internal ChannelDivinityOptionCatalog(
        IEnumerable<ChannelDivinityOptionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ChannelDivinityOptionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ChannelDivinityOptionDefinition definition in ordered)
        {
            ChannelDivinityOptionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ChannelDivinityOptionDefinition> All { get; }
    public int Count => All.Count;

    public ChannelDivinityOptionDefinition Get(ChannelDivinityOptionId id)
    {
        if (_byId.TryGetValue(
                id,
                out ChannelDivinityOptionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Channel Divinity option '{id}' does not exist in this " +
            "catalog.");
    }

    public bool TryGet(
        ChannelDivinityOptionId id,
        out ChannelDivinityOptionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ChannelDivinityOptionDefinition> definitions)
    {
        var ids = new HashSet<ChannelDivinityOptionId>();

        foreach (ChannelDivinityOptionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    "Duplicate Channel Divinity option ID " +
                    $"'{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
