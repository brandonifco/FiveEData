using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Spells.Concentration.Serialization;

internal static class ConcentrationRulesLoader
{
    public static ConcentrationRules LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static ConcentrationRules LoadFromJson(string json)
    {
        ConcentrationRulesData data =
            StrictJson.DeserializeObject<ConcentrationRulesData>(
                json,
                "Concentration rules");

        try
        {
            ConcentrationRules rules = Map(data);
            ConcentrationRulesValidator.EnsureValid(rules);
            return rules;
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            throw new InvalidDataException(
                "Invalid concentration rules definition.",
                exception);
        }
    }

    private static ConcentrationRules Map(ConcentrationRulesData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Concentration saving throw ability ID is required.",
                nameof(data));

        string[] endedByConditionIdValues = data.EndedByConditionIds
            ?? throw new ArgumentException(
                "Concentration ended-by condition IDs are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Concentration sources are required.",
                nameof(data));

        return new ConcentrationRules(
            new AbilityId(savingThrowAbilityIdValue),
            data.MinimumSavingThrowDC,
            data.DamageDivisorForSavingThrowDC,
            data.SeparateSavingThrowPerDamageSource,
            data.EndsWhenCastingAnotherConcentrationSpell,
            data.EndsOnDeath,
            data.CanBeEndedVoluntarilyWithoutAnAction,
            endedByConditionIdValues.Select(value => new ConditionId(value)),
            sourceData.Select(SourceReferenceDataMapper.Map));
    }
}
