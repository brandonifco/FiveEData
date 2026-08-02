using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Languages;

public sealed class LanguageDefinition
{
    internal LanguageDefinition(
        LanguageId id,
        string name,
        LanguageCategory category,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Category = category;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public LanguageId Id { get; }
    public string Name { get; }
    public LanguageCategory Category { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
