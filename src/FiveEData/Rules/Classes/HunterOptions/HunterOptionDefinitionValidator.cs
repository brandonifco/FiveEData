namespace FiveEData.Rules.Classes.HunterOptions;

internal static class HunterOptionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        HunterOptionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Hunter option ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Hunter option name must not be empty.");
        }

        if (definition.RequiredLevel is < 1 or > 20)
        {
            errors.Add(
                "Hunter option required level must be between 1 and 20.");
        }

        ValidatePositive(
            definition.SecondaryTargetRangeFeet,
            "secondary target range",
            errors);

        ValidatePositive(
            definition.ArmorClassBonusAgainstSubsequentAttacks,
            "armor class bonus",
            errors);

        ValidatePositive(
            definition.AttacksAnyNumberOfCreaturesWithinFeet,
            "multiattack range",
            errors);

        bool hasExtraDamage = definition.ExtraDamage is not null;
        bool hasExtraAttack = definition.GrantsExtraAttackAgainstDifferentTarget;

        if (definition.OncePerTurn && !hasExtraDamage && !hasExtraAttack)
        {
            errors.Add(
                "Hunter option cannot be once per turn without extra " +
                "damage or an extra attack to bound.");
        }

        if (definition.RequiresTargetBelowHitPointMaximum && !hasExtraDamage)
        {
            errors.Add(
                "Hunter option cannot require a target below its hit point " +
                "maximum without extra damage.");
        }

        if (definition.SecondaryTargetRangeFeet is not null && !hasExtraAttack)
        {
            errors.Add(
                "Hunter option cannot have a secondary target range without " +
                "an extra attack against a different target.");
        }

        bool hasMultiattackRange =
            definition.AttacksAnyNumberOfCreaturesWithinFeet is not null;
        bool hasMultiattackKind = definition.MultiattackKind is not null;

        if (hasMultiattackRange != hasMultiattackKind)
        {
            errors.Add(
                "Hunter option must have a multiattack range and a " +
                "multiattack kind together, or neither.");
        }

        if (definition.MultiattackKind is { } multiattackKind &&
            !Enum.IsDefined(multiattackKind))
        {
            errors.Add("Hunter option multiattack kind must be defined.");
        }

        bool hasSavingThrow = definition.SavingThrowAbilityId is not null;

        if (definition.NegatesDamageOnSuccessfulSave && !hasSavingThrow)
        {
            errors.Add(
                "Hunter option cannot negate damage on a successful save " +
                "without a saving throw.");
        }

        if (definition.HalfDamageOnFailedSave && !hasSavingThrow)
        {
            errors.Add(
                "Hunter option cannot deal half damage on a failed save " +
                "without a saving throw.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Hunter option must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(HunterOptionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Hunter option definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidatePositive(
        int? value,
        string description,
        List<string> errors)
    {
        if (value is { } amount && amount <= 0)
        {
            errors.Add(
                $"Hunter option {description} must be greater than zero.");
        }
    }
}
