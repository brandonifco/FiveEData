using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Classes.HunterOptions.Serialization;

internal static class HunterOptionDefinitionLoader
{
    public static IReadOnlyList<HunterOptionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<HunterOptionDefinition> LoadFromJson(
        string json)
    {
        HunterOptionDefinitionData[] data =
            StrictJson.DeserializeArray<HunterOptionDefinitionData>(
                json,
                "Hunter option");

        var definitions = new List<HunterOptionDefinition>(data.Length);
        var ids = new HashSet<HunterOptionId>();

        for (int index = 0; index < data.Length; index++)
        {
            HunterOptionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid hunter option definition at index {index}.");
            }

            HunterOptionDefinition definition;

            try
            {
                definition = Map(itemData);
                HunterOptionDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid hunter option definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate hunter option ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static HunterOptionDefinition Map(HunterOptionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new HunterOptionId(
            data.Id
            ?? throw new ArgumentException(
                "Hunter option ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Hunter option name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Hunter option sources are required.",
                nameof(data));

        DiceExpression? extraDamage =
            data.ExtraDamage is { } extraDamageData
                ? new DiceExpression(
                    extraDamageData.Count,
                    extraDamageData.Sides)
                : null;

        CreatureSizeId? minimumTargetSizeId =
            data.MinimumTargetSizeId is { } minimumTargetSizeIdValue
                ? new CreatureSizeId(minimumTargetSizeIdValue)
                : null;

        ConditionId? grantsAdvantageOnSavingThrowsAgainstConditionId =
            data.GrantsAdvantageOnSavingThrowsAgainstConditionId is
                { } advantageConditionIdValue
                ? new ConditionId(advantageConditionIdValue)
                : null;

        AbilityId? savingThrowAbilityId =
            data.SavingThrowAbilityId is { } savingThrowAbilityIdValue
                ? new AbilityId(savingThrowAbilityIdValue)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new HunterOptionDefinition(
            id: id,
            name: name,
            requiredLevel: data.RequiredLevel,
            extraDamage: extraDamage,
            oncePerTurn: data.OncePerTurn,
            requiresTargetBelowHitPointMaximum:
                data.RequiresTargetBelowHitPointMaximum,
            minimumTargetSizeId: minimumTargetSizeId,
            grantsExtraAttackAgainstDifferentTarget:
                data.GrantsExtraAttackAgainstDifferentTarget,
            secondaryTargetRangeFeet: data.SecondaryTargetRangeFeet,
            imposesDisadvantageOnOpportunityAttacksAgainstYou:
                data.ImposesDisadvantageOnOpportunityAttacksAgainstYou,
            armorClassBonusAgainstSubsequentAttacks:
                data.ArmorClassBonusAgainstSubsequentAttacks,
            grantsAdvantageOnSavingThrowsAgainstConditionId:
                grantsAdvantageOnSavingThrowsAgainstConditionId,
            attacksAnyNumberOfCreaturesWithinFeet:
                data.AttacksAnyNumberOfCreaturesWithinFeet,
            multiattackKind: data.MultiattackKind,
            savingThrowAbilityId: savingThrowAbilityId,
            negatesDamageOnSuccessfulSave: data.NegatesDamageOnSuccessfulSave,
            halfDamageOnFailedSave: data.HalfDamageOnFailedSave,
            halvesAttackDamageAsReaction: data.HalvesAttackDamageAsReaction,
            sources: sources);
    }
}
