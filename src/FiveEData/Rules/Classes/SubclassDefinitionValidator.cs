using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes;

internal static class SubclassDefinitionValidator
{
    public static IReadOnlyList<string> Validate(SubclassDefinition subclass)
    {
        ArgumentNullException.ThrowIfNull(subclass);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(subclass.Id.Value))
        {
            errors.Add("Subclass ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(subclass.Name))
        {
            errors.Add("Subclass name must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(subclass.ClassId.Value))
        {
            errors.Add("Subclass class ID must not be empty.");
        }

        if (subclass.ChosenAtLevel is < 1 or > 20)
        {
            errors.Add(
                "Subclass chosen-at level must be between 1 and 20.");
        }

        if (subclass.Sources.Count == 0)
        {
            errors.Add("Subclass must have at least one source reference.");
        }

        var seenLevelFeatures = new HashSet<(int Level, RuleId FeatureRuleId)>();

        foreach (ClassLevelFeature feature in subclass.LevelFeatures)
        {
            if (!seenLevelFeatures.Add((feature.Level, feature.FeatureRuleId)))
            {
                errors.Add(
                    $"Subclass level feature '{feature.FeatureRuleId}' is " +
                    $"duplicated at level {feature.Level}.");
            }
        }

        return errors;
    }

    public static void EnsureValid(SubclassDefinition subclass)
    {
        IReadOnlyList<string> errors = Validate(subclass);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Subclass definition '{subclass.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
