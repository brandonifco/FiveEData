using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Characters.Encumbrance.Serialization;

internal static class EncumbranceRulesLoader
{
    public static EncumbranceRules LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static EncumbranceRules LoadFromJson(string json)
    {
        EncumbranceRulesData data =
            StrictJson.DeserializeObject<EncumbranceRulesData>(
                json,
                "Encumbrance rules");

        try
        {
            EncumbranceRules rules = Map(data);
            EncumbranceRulesValidator.EnsureValid(rules);
            return rules;
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            throw new InvalidDataException(
                "Invalid encumbrance rules definition.",
                exception);
        }
    }

    private static EncumbranceRules Map(EncumbranceRulesData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        CarryingCapacitySizeMultiplierGrantData[] sizeMultiplierData =
            data.SizeCarryingCapacityMultipliers
            ?? throw new ArgumentException(
                "Encumbrance size carrying capacity multipliers are " +
                "required.",
                nameof(data));

        string[] disadvantageAbilityIdValues =
            data.HeavilyEncumberedDisadvantageAbilityIds
            ?? throw new ArgumentException(
                "Encumbrance heavily encumbered disadvantage ability IDs " +
                "are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Encumbrance sources are required.",
                nameof(data));

        IEnumerable<CarryingCapacitySizeMultiplierGrant> sizeMultipliers =
            sizeMultiplierData.Select(
                (grant, index) =>
                    grant is null
                        ? throw new ArgumentException(
                            $"Encumbrance size carrying capacity " +
                            $"multiplier at index {index} is required.",
                            nameof(data))
                        : new CarryingCapacitySizeMultiplierGrant(
                            new CreatureSizeId(
                                grant.SizeId
                                ?? throw new ArgumentException(
                                    $"Encumbrance size carrying capacity " +
                                    $"multiplier at index {index} requires " +
                                    $"a size ID.",
                                    nameof(data))),
                            grant.Multiplier));

        return new EncumbranceRules(
            sizeMultipliers,
            data.EncumberedCarryingCapacityMultiplier,
            data.EncumberedSpeedReductionFeet,
            data.HeavilyEncumberedCarryingCapacityMultiplier,
            data.HeavilyEncumberedSpeedReductionFeet,
            disadvantageAbilityIdValues.Select(
                value => new AbilityId(value)),
            sourceData.Select(SourceReferenceDataMapper.Map));
    }
}
