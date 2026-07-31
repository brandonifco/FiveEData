using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Armor.Serialization;

internal static class ArmorDefinitionLoader
{
    public static IReadOnlyList<ArmorDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ArmorDefinition> LoadFromJson(string json)
    {
        ArmorDefinitionData[] data =
            StrictJson.DeserializeArray<ArmorDefinitionData>(
                json,
                "Armor");

        var definitions = new List<ArmorDefinition>(data.Length);
        var ids = new HashSet<ArmorId>();

        for (int index = 0; index < data.Length; index++)
        {
            ArmorDefinitionData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid armor definition at index {index}.");
            }

            ArmorDefinition definition;

            try
            {
                definition = Map(itemData);
                ArmorDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid armor definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate armor ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ArmorDefinition Map(ArmorDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ArmorId(
            data.Id
            ?? throw new ArgumentException(
                "Armor ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Armor name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Armor cost is required.",
                nameof(data));

        WeightData weight = data.Weight
            ?? throw new ArgumentException(
                "Armor weight is required.",
                nameof(data));

        ArmorClassFormulaData armorClass = data.ArmorClass
            ?? throw new ArgumentException(
                "Armor Class formula is required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Armor sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ArmorDefinition(
            id,
            name,
            data.Category,
            new Money(cost.CopperPieces),
            new Weight(weight.Pounds),
            new ArmorClassFormula(
                armorClass.BaseArmorClass,
                armorClass.IncludesDexterityModifier,
                armorClass.MaximumDexterityModifier),
            data.MinimumStrengthForFullSpeed,
            data.ImposesStealthDisadvantage,
            sources);
    }
}
