using FiveEData.Rules.Classes;

namespace FiveEData.Rules.Catalog;

internal sealed class ClassDefinitionSet
{
    public ClassDefinitionSet(
        IReadOnlyList<ClassDefinition> classes,
        IReadOnlyList<SubclassDefinition> subclasses)
    {
        ArgumentNullException.ThrowIfNull(classes);
        ArgumentNullException.ThrowIfNull(subclasses);

        Classes = classes;
        Subclasses = subclasses;
    }

    public IReadOnlyList<ClassDefinition> Classes { get; }
    public IReadOnlyList<SubclassDefinition> Subclasses { get; }
}
