namespace FiveEData.Rules.Classes.EldritchInvocations;

internal static class EldritchInvocationDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        EldritchInvocationDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Eldritch invocation ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Eldritch invocation name must not be empty.");
        }

        if (definition.RequiredMinimumLevel is { } requiredMinimumLevel &&
            requiredMinimumLevel is < 1 or > 20)
        {
            errors.Add(
                "Eldritch invocation required minimum level must be " +
                "between 1 and 20.");
        }

        if (definition.RequiresPactBoon is { } requiresPactBoon &&
            !Enum.IsDefined(requiresPactBoon))
        {
            errors.Add(
                "Eldritch invocation required Pact Boon must be a " +
                "defined value.");
        }

        bool hasGrantedSpell = definition.GrantedSpellId is not null;
        bool hasCastingFrequency = definition.CastingFrequency is not null;

        if (hasGrantedSpell != hasCastingFrequency)
        {
            errors.Add(
                "Eldritch invocation must have a granted spell and a " +
                "casting frequency together, or neither.");
        }

        if (definition.CastingFrequency is { } castingFrequency &&
            !Enum.IsDefined(castingFrequency))
        {
            errors.Add(
                "Eldritch invocation casting frequency must be a " +
                "defined value.");
        }

        if (definition.WaivesMaterialComponents && !hasGrantedSpell)
        {
            errors.Add(
                "Eldritch invocation cannot waive material components " +
                "without a granted spell.");
        }

        if (definition.ExtraDamageTypeId is not null &&
            !definition.AddsSpellcastingModifierToDamage)
        {
            errors.Add(
                "Eldritch invocation cannot have an extra damage type " +
                "without adding the spellcasting modifier to damage.");
        }

        if (definition.SkillProficiencyIds.Distinct().Count() !=
            definition.SkillProficiencyIds.Count)
        {
            errors.Add(
                "Eldritch invocation skill proficiencies must not " +
                "contain duplicates.");
        }

        if (definition.DarknessVisionRangeFeet is
                { } darknessVisionRangeFeet &&
            darknessVisionRangeFeet <= 0)
        {
            errors.Add(
                "Eldritch invocation darkness vision range must be " +
                "greater than zero.");
        }

        if (definition.TrueSightRangeFeet is { } trueSightRangeFeet &&
            trueSightRangeFeet <= 0)
        {
            errors.Add(
                "Eldritch invocation true sight range must be greater " +
                "than zero.");
        }

        if (definition.EldritchBlastRangeFeet is
                { } eldritchBlastRangeFeet &&
            eldritchBlastRangeFeet <= 0)
        {
            errors.Add(
                "Eldritch invocation eldritch blast range must be " +
                "greater than zero.");
        }

        if (definition.EldritchBlastPushDistanceFeet is
                { } eldritchBlastPushDistanceFeet &&
            eldritchBlastPushDistanceFeet <= 0)
        {
            errors.Add(
                "Eldritch invocation eldritch blast push distance must " +
                "be greater than zero.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Eldritch invocation must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(EldritchInvocationDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Eldritch invocation definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
