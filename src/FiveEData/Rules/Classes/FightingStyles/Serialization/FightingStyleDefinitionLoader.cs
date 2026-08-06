using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.FightingStyles.Serialization;

internal static class FightingStyleDefinitionLoader
{
    public static IReadOnlyList<FightingStyleDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<FightingStyleDefinition> LoadFromJson(
        string json)
    {
        FightingStyleDefinitionData[] data =
            StrictJson.DeserializeArray<FightingStyleDefinitionData>(
                json,
                "Fighting style");

        var definitions =
            new List<FightingStyleDefinition>(data.Length);
        var ids = new HashSet<FightingStyleId>();

        for (int index = 0; index < data.Length; index++)
        {
            FightingStyleDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid fighting style definition at index {index}.");
            }

            FightingStyleDefinition definition;

            try
            {
                definition = Map(itemData);
                FightingStyleDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid fighting style definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate fighting style ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static FightingStyleDefinition Map(
        FightingStyleDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new FightingStyleId(
            data.Id
            ?? throw new ArgumentException(
                "Fighting style ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Fighting style name is required.",
                nameof(data));

        string[] availableToClassIdValues =
            data.AvailableToClassIds
            ?? throw new ArgumentException(
                "Fighting style available class IDs are required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Fighting style sources are required.",
                nameof(data));

        ClassId[] availableToClassIds = availableToClassIdValues
            .Select(value => new ClassId(value))
            .ToArray();

        FightingStyleRollBonus? rollBonus =
            data.RollBonus is { } rollBonusData
                ? new FightingStyleRollBonus(
                    rollBonusData.Target,
                    rollBonusData.Amount,
                    rollBonusData.WeaponRequirement)
                : null;

        FightingStyleDamageDieReroll? damageDieReroll =
            data.DamageDieReroll is { } rerollData
                ? new FightingStyleDamageDieReroll(
                    rerollData.RerollAtOrBelowValue,
                    rerollData.WeaponRequirement)
                : null;

        FightingStyleReaction? reaction =
            data.Reaction is { } reactionData
                ? new FightingStyleReaction(
                    new Distance(reactionData.RangeFeet),
                    reactionData.RequiresShield)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new FightingStyleDefinition(
            id,
            name,
            availableToClassIds,
            rollBonus,
            data.ArmorClassBonus,
            damageDieReroll,
            reaction,
            data.GrantsOffHandAbilityModifierDamage,
            sources);
    }
}
