using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Races.BreathWeapon;
using FiveEData.Rules.Creatures.Races.Lucky;
using FiveEData.Rules.Creatures.Races.Lucky.Serialization;
using FiveEData.Rules.Creatures.Races.RelentlessEndurance;
using FiveEData.Rules.Creatures.Races.RelentlessEndurance.Serialization;
using FiveEData.Rules.Creatures.Races.SavageAttacks;
using FiveEData.Rules.Creatures.Races.SavageAttacks.Serialization;
using FiveEData.Rules.Creatures.Races.BreathWeapon.Serialization;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Creatures.Races.Serialization;

internal static class RaceDefinitionLoader
{
    public static IReadOnlyList<RaceDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<RaceDefinition> LoadFromJson(string json)
    {
        RaceDefinitionData[] data =
            StrictJson.DeserializeArray<RaceDefinitionData>(json, "Race");

        var races = new List<RaceDefinition>(data.Length);
        var ids = new HashSet<RaceId>();

        for (int index = 0; index < data.Length; index++)
        {
            RaceDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid race definition at index {index}.");
            }

            RaceDefinition race;

            try
            {
                race = Map(itemData);
                RaceDefinitionValidator.EnsureValid(race);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid race definition at {identity}.",
                    exception);
            }

            if (!ids.Add(race.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate race ID '{race.Id}'.");
            }

            races.Add(race);
        }

        return races;
    }

    private static RaceDefinition Map(RaceDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new RaceId(
            data.Id
            ?? throw new ArgumentException(
                "Race ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Race name is required.",
                nameof(data));

        var size = new CreatureSizeId(
            data.Size
            ?? throw new ArgumentException(
                "Race size is required.",
                nameof(data)));

        RaceAbilityScoreIncreaseData[] abilityScoreIncreaseData =
            data.AbilityScoreIncreases
            ?? throw new ArgumentException(
                "Race ability score increases are required.",
                nameof(data));

        string[] languageIdValues = data.LanguageIds
            ?? throw new ArgumentException(
                "Race language IDs are required.",
                nameof(data));

        string[] traitRuleIdValues = data.TraitRuleIds
            ?? throw new ArgumentException(
                "Race trait rule IDs are required.",
                nameof(data));

        string[] resistedDamageTypeIdValues = data.ResistedDamageTypeIds
            ?? throw new ArgumentException(
                "Race resisted damage type IDs are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Race sources are required.",
                nameof(data));

        RaceAbilityScoreIncrease[] abilityScoreIncreases =
            abilityScoreIncreaseData
                .Select(MapAbilityScoreIncrease)
                .ToArray();

        LanguageId[] languageIds = languageIdValues
            .Select(value => new LanguageId(value))
            .ToArray();

        RuleId[] traitRuleIds = traitRuleIdValues
            .Select(value => new RuleId(value))
            .ToArray();

        DamageTypeId[] resistedDamageTypeIds = resistedDamageTypeIdValues
            .Select(value => new DamageTypeId(value))
            .ToArray();

        BreathWeaponProgressionDetail? breathWeaponProgression =
            data.BreathWeaponProgression is { } breathWeaponProgressionData
                ? BreathWeaponProgressionDetailDataMapper.Map(
                    breathWeaponProgressionData)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        SavageAttacksDetail? savageAttacks =
            data.SavageAttacks is { } savageAttacksData
                ? SavageAttacksDetailDataMapper.Map(savageAttacksData)
                : null;

        RelentlessEnduranceDetail? relentlessEndurance =
            data.RelentlessEndurance is { } relentlessEnduranceData
                ? RelentlessEnduranceDetailDataMapper.Map(
                    relentlessEnduranceData)
                : null;

        LuckyDetail? lucky =
            data.Lucky is { } luckyData
                ? LuckyDetailDataMapper.Map(luckyData)
                : null;

        return new RaceDefinition(
            id,
            name,
            size,
            new Distance(data.SpeedFeet),
            abilityScoreIncreases,
            data.ChoosableAbilityScoreIncreaseCount,
            languageIds,
            data.AdditionalLanguageChoiceCount,
            traitRuleIds,
            data.DarkvisionRangeFeet,
            resistedDamageTypeIds,
            data.TranceDurationHours,
            breathWeaponProgression,
            savageAttacks,
            relentlessEndurance,
            lucky,
            sources);
    }

    internal static RaceAbilityScoreIncrease MapAbilityScoreIncrease(
        RaceAbilityScoreIncreaseData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var abilityId = new AbilityId(
            data.AbilityId
            ?? throw new ArgumentException(
                "Ability score increase ability ID is required.",
                nameof(data)));

        return new RaceAbilityScoreIncrease(abilityId, data.Bonus);
    }
}
