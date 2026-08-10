using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Classes.TotemWarriorOptions.Serialization;

internal static class TotemWarriorOptionDefinitionLoader
{
    public static IReadOnlyList<TotemWarriorOptionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<TotemWarriorOptionDefinition> LoadFromJson(
        string json)
    {
        TotemWarriorOptionDefinitionData[] data =
            StrictJson.DeserializeArray<TotemWarriorOptionDefinitionData>(
                json,
                "Totem warrior option");

        var definitions = new List<TotemWarriorOptionDefinition>(data.Length);
        var ids = new HashSet<TotemWarriorOptionId>();

        for (int index = 0; index < data.Length; index++)
        {
            TotemWarriorOptionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid totem warrior option definition at index " +
                    $"{index}.");
            }

            TotemWarriorOptionDefinition definition;

            try
            {
                definition = Map(itemData);
                TotemWarriorOptionDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid totem warrior option definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate totem warrior option ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static TotemWarriorOptionDefinition Map(
        TotemWarriorOptionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new TotemWarriorOptionId(
            data.Id
            ?? throw new ArgumentException(
                "Totem warrior option ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Totem warrior option name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Totem warrior option sources are required.",
                nameof(data));

        DamageTypeId? resistsAllDamageExceptTypeId =
            data.ResistsAllDamageExceptTypeId is
                { } resistsAllDamageExceptTypeIdValue
                ? new DamageTypeId(resistsAllDamageExceptTypeIdValue)
                : null;

        TravelPaceId? tracksAtTravelPaceId =
            data.TracksAtTravelPaceId is { } tracksAtTravelPaceIdValue
                ? new TravelPaceId(tracksAtTravelPaceIdValue)
                : null;

        TravelPaceId? movesStealthilyAtTravelPaceId =
            data.MovesStealthilyAtTravelPaceId is
                { } movesStealthilyAtTravelPaceIdValue
                ? new TravelPaceId(movesStealthilyAtTravelPaceIdValue)
                : null;

        ConditionId? imposedConditionId =
            data.ImposedConditionId is { } imposedConditionIdValue
                ? new ConditionId(imposedConditionIdValue)
                : null;

        CreatureSizeId? maximumTargetSizeId =
            data.MaximumTargetSizeId is { } maximumTargetSizeIdValue
                ? new CreatureSizeId(maximumTargetSizeIdValue)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new TotemWarriorOptionDefinition(
            id: id,
            name: name,
            requiredLevel: data.RequiredLevel,
            requiresRaging: data.RequiresRaging,
            requiresNotWearingHeavyArmor: data.RequiresNotWearingHeavyArmor,
            resistsAllDamageExceptTypeId: resistsAllDamageExceptTypeId,
            imposesDisadvantageOnOpportunityAttacksAgainstYou:
                data.ImposesDisadvantageOnOpportunityAttacksAgainstYou,
            grantsDashAsBonusAction: data.GrantsDashAsBonusAction,
            grantsAlliesAdvantageOnMeleeAttacksWithinFeet:
                data.GrantsAlliesAdvantageOnMeleeAttacksWithinFeet,
            doublesCarryingCapacity: data.DoublesCarryingCapacity,
            grantsAdvantageOnStrengthChecksToMoveObjects:
                data.GrantsAdvantageOnStrengthChecksToMoveObjects,
            clearSightRangeFeet: data.ClearSightRangeFeet,
            clearSightDetailEquivalentRangeFeet:
                data.ClearSightDetailEquivalentRangeFeet,
            ignoresDimLightPerceptionDisadvantage:
                data.IgnoresDimLightPerceptionDisadvantage,
            tracksAtTravelPaceId: tracksAtTravelPaceId,
            movesStealthilyAtTravelPaceId: movesStealthilyAtTravelPaceId,
            imposesDisadvantageOnAttacksAgainstOthersWithinFeet:
                data.ImposesDisadvantageOnAttacksAgainstOthersWithinFeet,
            grantsFlyingSpeedEqualToWalkingSpeed:
                data.GrantsFlyingSpeedEqualToWalkingSpeed,
            imposedConditionId: imposedConditionId,
            maximumTargetSizeId: maximumTargetSizeId,
            imposedConditionRequiresBonusAction:
                data.ImposedConditionRequiresBonusAction,
            sources: sources);
    }
}
