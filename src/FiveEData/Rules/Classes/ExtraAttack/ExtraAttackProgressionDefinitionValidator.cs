namespace FiveEData.Rules.Classes.ExtraAttack;

internal static class ExtraAttackProgressionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        ExtraAttackProgressionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Extra Attack progression ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Extra Attack progression name must not be empty.");
        }

        if (definition.Grants.Count == 0)
        {
            errors.Add(
                "Extra Attack progression must grant at least one " +
                "attack increase.");
        }

        var seenLevels = new HashSet<int>();
        int previousAttackCount = 1;
        int previousLevel = 0;

        foreach (
            ExtraAttackGrant grant
            in definition.Grants.OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Extra Attack progression character level " +
                    $"{grant.CharacterLevel} is duplicated.");
            }

            if (grant.CharacterLevel <= previousLevel)
            {
                previousLevel = grant.CharacterLevel;
                continue;
            }

            if (grant.AttackCount <= previousAttackCount)
            {
                errors.Add(
                    "Extra Attack progression attack count at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "attack count at the previous grant level.");
            }

            previousAttackCount = grant.AttackCount;
            previousLevel = grant.CharacterLevel;
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Extra Attack progression must have at least one " +
                "source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        ExtraAttackProgressionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "Extra Attack progression definition " +
            $"'{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
