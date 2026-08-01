using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Expenses.FoodAndLodging.Serialization;

internal static class FoodDrinkDefinitionLoader
{
    public static IReadOnlyList<FoodDrinkDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<FoodDrinkDefinition> LoadFromJson(
        string json)
    {
        FoodDrinkDefinitionData[] data =
            StrictJson.DeserializeArray<FoodDrinkDefinitionData>(
                json,
                "Food and drink");

        var definitions =
            new List<FoodDrinkDefinition>(data.Length);
        var ids = new HashSet<FoodDrinkId>();

        for (int index = 0; index < data.Length; index++)
        {
            FoodDrinkDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid food-and-drink definition at index {index}.");
            }

            FoodDrinkDefinition definition;

            try
            {
                definition = Map(itemData);
                FoodDrinkDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid food-and-drink definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate food-and-drink ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static FoodDrinkDefinition Map(
        FoodDrinkDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new FoodDrinkId(
            data.Id
            ?? throw new ArgumentException(
                "Food-and-drink ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Food-and-drink name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Food-and-drink cost is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Food-and-drink special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Food-and-drink sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new FoodDrinkDefinition(
            id,
            name,
            new Money(cost.CopperPieces),
            data.PricingUnit,
            specialRuleIds,
            sources);
    }
}
