using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Conditions.Serialization;

internal static class ConditionDefinitionLoader
{
    public static IReadOnlyList<ConditionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ConditionDefinition> LoadFromJson(
        string json)
    {
        ConditionDefinitionData[] data =
            StrictJson.DeserializeArray<ConditionDefinitionData>(
                json,
                "Condition");

        var definitions =
            new List<ConditionDefinition>(data.Length);
        var ids = new HashSet<ConditionId>();

        for (int index = 0; index < data.Length; index++)
        {
            ConditionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid condition definition at index {index}.");
            }

            ConditionDefinition definition;

            try
            {
                definition = Map(itemData);
                ConditionDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid condition definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate condition ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ConditionDefinition Map(
        ConditionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ConditionId(
            data.Id
            ?? throw new ArgumentException(
                "Condition ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Condition name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Condition sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        ExhaustionEffectDetail? exhaustionEffect =
            data.ExhaustionEffect is { } exhaustionEffectData
                ? MapExhaustionEffect(exhaustionEffectData)
                : null;

        return new ConditionDefinition(
            id: id,
            name: name,
            preventsActionsAndReactions:
                data.PreventsActionsAndReactions,
            preventsMovement: data.PreventsMovement,
            onlyMovementOptionIsToCrawl:
                data.OnlyMovementOptionIsToCrawl,
            speedBecomesZero: data.SpeedBecomesZero,
            ignoresBonusesToSpeed: data.IgnoresBonusesToSpeed,
            speechRestriction: data.SpeechRestriction,
            unawareOfSurroundings: data.UnawareOfSurroundings,
            automaticallyFailsStrengthAndDexteritySavingThrows:
                data.AutomaticallyFailsStrengthAndDexteritySavingThrows,
            dexteritySavingThrowsHaveDisadvantage:
                data.DexteritySavingThrowsHaveDisadvantage,
            automaticallyFailsAbilityChecksRequiringSight:
                data.AutomaticallyFailsAbilityChecksRequiringSight,
            automaticallyFailsAbilityChecksRequiringHearing:
                data.AutomaticallyFailsAbilityChecksRequiringHearing,
            ownAbilityChecksHaveDisadvantage:
                data.OwnAbilityChecksHaveDisadvantage,
            attackRollsAgainstTheCreature:
                data.AttackRollsAgainstTheCreature,
            theCreaturesOwnAttackRolls: data.TheCreaturesOwnAttackRolls,
            anyHitIsACriticalHitIfAttackerIsWithinFiveFeet:
                data.AnyHitIsACriticalHitIfAttackerIsWithinFiveFeet,
            requiresSourceInLineOfSightForRollEffects:
                data.RequiresSourceInLineOfSightForRollEffects,
            cannotWillinglyMoveCloserToSource:
                data.CannotWillinglyMoveCloserToSource,
            cannotAttackOrTargetSourceWithHarmfulEffects:
                data.CannotAttackOrTargetSourceWithHarmfulEffects,
            sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature:
                data
                    .SourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature,
            endsIfSourceCreatureIsIncapacitated:
                data.EndsIfSourceCreatureIsIncapacitated,
            resistantToAllDamage: data.ResistantToAllDamage,
            immuneToPoisonAndDisease: data.ImmuneToPoisonAndDisease,
            weightMultiplier: data.WeightMultiplier,
            dropsHeldItemsAndFallsProne: data.DropsHeldItemsAndFallsProne,
            heavilyObscuredForHidingPurposes:
                data.HeavilyObscuredForHidingPurposes,
            exhaustionEffect: exhaustionEffect,
            sources: sources);
    }

    private static ExhaustionEffectDetail MapExhaustionEffect(
        ExhaustionEffectDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ExhaustionLevelEffect[] levelEffects =
            data.LevelEffects
            ?? throw new ArgumentException(
                "Exhaustion effect level effects are required.",
                nameof(data));

        return new ExhaustionEffectDetail(
            levelEffects,
            data.RecoversOneLevelPerLongRest,
            data.RecoveryRequiresFoodAndDrink);
    }
}
