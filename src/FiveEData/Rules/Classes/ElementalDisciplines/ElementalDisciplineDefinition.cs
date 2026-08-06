using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.ElementalDisciplines;

public sealed class ElementalDisciplineDefinition
{
    internal ElementalDisciplineDefinition(
        ElementalDisciplineId id,
        string name,
        int? kiPointCost,
        int? requiredMinimumLevel,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        KiPointCost = kiPointCost;
        RequiredMinimumLevel = requiredMinimumLevel;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ElementalDisciplineId Id { get; }
    public string Name { get; }
    public int? KiPointCost { get; }
    public int? RequiredMinimumLevel { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
