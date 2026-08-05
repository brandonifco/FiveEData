using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes;

public sealed class SubclassDefinition
{
    internal SubclassDefinition(
        SubclassId id,
        string name,
        ClassId classId,
        int chosenAtLevel,
        IEnumerable<ClassLevelFeature> levelFeatures,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(levelFeatures);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        ClassId = classId;
        ChosenAtLevel = chosenAtLevel;
        LevelFeatures = Array.AsReadOnly(levelFeatures.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SubclassId Id { get; }
    public string Name { get; }
    public ClassId ClassId { get; }
    public int ChosenAtLevel { get; }
    public IReadOnlyList<ClassLevelFeature> LevelFeatures { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
