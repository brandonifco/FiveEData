namespace FiveEData.Rules.Creatures.Skills;

internal static class SkillDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        SkillDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Skill ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Skill name must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(
                definition.NormallyAssociatedAbilityId.Value))
        {
            errors.Add(
                "Skill normally associated ability ID " +
                "must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Skill must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        SkillDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Skill definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
