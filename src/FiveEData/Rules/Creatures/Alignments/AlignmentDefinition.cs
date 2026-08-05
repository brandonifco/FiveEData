using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Alignments;

public sealed class AlignmentDefinition
{
    internal AlignmentDefinition(
        AlignmentId id,
        string name,
        AlignmentEthic ethic,
        AlignmentMorality morality,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Ethic = ethic;
        Morality = morality;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public AlignmentId Id { get; }
    public string Name { get; }
    public AlignmentEthic Ethic { get; }
    public AlignmentMorality Morality { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
