using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Expenses.FoodAndLodging.Serialization;

internal static class LifestyleHospitalityCostDefinitionLoader
{
    public static IReadOnlyList<
        LifestyleHospitalityCostDefinition> LoadFromFile(
            string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<
        LifestyleHospitalityCostDefinition> LoadFromJson(
            string json)
    {
        LifestyleHospitalityCostDefinitionData[] data =
            StrictJson.DeserializeArray<
                LifestyleHospitalityCostDefinitionData>(
                    json,
                    "Lifestyle hospitality cost");

        var definitions =
            new List<LifestyleHospitalityCostDefinition>(
                data.Length);
        var lifestyleIds = new HashSet<LifestyleId>();

        for (int index = 0; index < data.Length; index++)
        {
            LifestyleHospitalityCostDefinitionData? itemData =
                data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    "Invalid lifestyle hospitality-cost " +
                    $"definition at index {index}.");
            }

            LifestyleHospitalityCostDefinition definition;

            try
            {
                definition = Map(itemData);
                LifestyleHospitalityCostDefinitionValidator
                    .EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is
                    ArgumentException or
                    InvalidOperationException)
            {
                string identity =
                    string.IsNullOrWhiteSpace(
                        itemData.LifestyleId)
                        ? $"index {index}"
                        : $"'{itemData.LifestyleId}'";

                throw new InvalidDataException(
                    "Invalid lifestyle hospitality-cost " +
                    $"definition at {identity}.",
                    exception);
            }

            if (!lifestyleIds.Add(definition.LifestyleId))
            {
                throw new InvalidDataException(
                    "Duplicate hospitality-cost lifestyle ID " +
                    $"'{definition.LifestyleId}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static LifestyleHospitalityCostDefinition Map(
        LifestyleHospitalityCostDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var lifestyleId = new LifestyleId(
            data.LifestyleId
            ?? throw new ArgumentException(
                "Hospitality-cost lifestyle ID is required.",
                nameof(data)));

        MoneyData innStayCost = data.InnStayCostPerDay
            ?? throw new ArgumentException(
                "Hospitality inn-stay cost per day is required.",
                nameof(data));

        MoneyData mealsCost = data.MealsCostPerDay
            ?? throw new ArgumentException(
                "Hospitality meals cost per day is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Hospitality special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Hospitality sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new LifestyleHospitalityCostDefinition(
            lifestyleId,
            new Money(innStayCost.CopperPieces),
            new Money(mealsCost.CopperPieces),
            specialRuleIds,
            sources);
    }
}
