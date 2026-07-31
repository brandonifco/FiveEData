using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Common;

public sealed class RuleDefinition
{
    internal RuleDefinition(
        RuleId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public RuleId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
