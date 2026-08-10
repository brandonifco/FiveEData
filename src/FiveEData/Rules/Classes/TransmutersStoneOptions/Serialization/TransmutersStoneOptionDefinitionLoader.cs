using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.TransmutersStoneOptions.Serialization;

internal static class TransmutersStoneOptionDefinitionLoader
{
    public static IReadOnlyList<TransmutersStoneOptionDefinition>
        LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<TransmutersStoneOptionDefinition>
        LoadFromJson(string json)
    {
        TransmutersStoneOptionDefinitionData[] data =
            StrictJson.DeserializeArray<
                TransmutersStoneOptionDefinitionData>(
                json,
                "Transmuter\u0027s stone option");

        var definitions =
            new List<TransmutersStoneOptionDefinition>(data.Length);
        var ids = new HashSet<TransmutersStoneOptionId>();

        for (int index = 0; index < data.Length; index++)
        {
            TransmutersStoneOptionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid transmuter\u0027s stone option definition at " +
                    $"index {index}.");
            }

            TransmutersStoneOptionDefinition definition;

            try
            {
                definition = Map(itemData);
                TransmutersStoneOptionDefinitionValidator.EnsureValid(
                    definition);
            }
            catch (Exception exception)
                when (exception is
                    ArgumentException or
                    InvalidOperationException)
            {
                string identity =
                    string.IsNullOrWhiteSpace(itemData.Id)
                        ? $"index {index}"
                        : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid transmuter\u0027s stone option definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate transmuter\u0027s stone option ID " +
                    $"'{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static TransmutersStoneOptionDefinition Map(
        TransmutersStoneOptionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new TransmutersStoneOptionId(
            data.Id
            ?? throw new ArgumentException(
                "Transmuter\u0027s stone option ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Transmuter\u0027s stone option name is required.",
                nameof(data));

        string[] choosableResistedDamageTypeIdValues =
            data.ChoosableResistedDamageTypeIds
            ?? throw new ArgumentException(
                "Transmuter\u0027s stone option choosable resisted damage " +
                "types are required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Transmuter\u0027s stone option sources are required.",
                nameof(data));

        AbilityId? savingThrowProficiencyAbilityId =
            data.SavingThrowProficiencyAbilityId is
                { } savingThrowProficiencyAbilityIdValue
                ? new AbilityId(savingThrowProficiencyAbilityIdValue)
                : null;

        DamageTypeId[] choosableResistedDamageTypeIds =
            choosableResistedDamageTypeIdValues
                .Select(value => new DamageTypeId(value))
                .ToArray();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new TransmutersStoneOptionDefinition(
            id: id,
            name: name,
            darkvisionRangeFeet: data.DarkvisionRangeFeet,
            speedBonusFeet: data.SpeedBonusFeet,
            requiresUnencumbered: data.RequiresUnencumbered,
            savingThrowProficiencyAbilityId: savingThrowProficiencyAbilityId,
            choosableResistedDamageTypeIds: choosableResistedDamageTypeIds,
            sources: sources);
    }
}
