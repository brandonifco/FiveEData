namespace FiveEData.Rules.Classes.BattleMasterManeuvers;

internal static class BattleMasterManeuverDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        BattleMasterManeuverDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Battle Master maneuver ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Battle Master maneuver name must not be empty.");
        }

        if (!Enum.IsDefined(definition.EffectTarget))
        {
            errors.Add(
                "Battle Master maneuver effect target must be a defined " +
                "value.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Battle Master maneuver must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(BattleMasterManeuverDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Battle Master maneuver definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
