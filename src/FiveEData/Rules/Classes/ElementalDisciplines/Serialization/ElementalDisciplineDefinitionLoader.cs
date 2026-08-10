using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Classes.ElementalDisciplines.Serialization;

internal static class ElementalDisciplineDefinitionLoader
{
    public static IReadOnlyList<ElementalDisciplineDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ElementalDisciplineDefinition> LoadFromJson(
        string json)
    {
        ElementalDisciplineDefinitionData[] data =
            StrictJson.DeserializeArray<ElementalDisciplineDefinitionData>(
                json,
                "Elemental discipline");

        var definitions =
            new List<ElementalDisciplineDefinition>(data.Length);
        var ids = new HashSet<ElementalDisciplineId>();

        for (int index = 0; index < data.Length; index++)
        {
            ElementalDisciplineDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid elemental discipline definition at index " +
                    $"{index}.");
            }

            ElementalDisciplineDefinition definition;

            try
            {
                definition = Map(itemData);
                ElementalDisciplineDefinitionValidator.EnsureValid(
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
                    $"Invalid elemental discipline definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate elemental discipline ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ElementalDisciplineDefinition Map(
        ElementalDisciplineDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ElementalDisciplineId(
            data.Id
            ?? throw new ArgumentException(
                "Elemental discipline ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Elemental discipline name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Elemental discipline sources are required.",
                nameof(data));

        SpellId? grantedSpellId =
            data.GrantedSpellId is { } grantedSpellIdValue
                ? new SpellId(grantedSpellIdValue)
                : null;

        AbilityId? savingThrowAbilityId =
            data.SavingThrowAbilityId is { } savingThrowAbilityIdValue
                ? new AbilityId(savingThrowAbilityIdValue)
                : null;

        DiceExpression? baseDamage =
            data.BaseDamage is { } baseDamageData
                ? new DiceExpression(
                    baseDamageData.Count,
                    baseDamageData.Sides)
                : null;

        DamageTypeId? baseDamageTypeId =
            data.BaseDamageTypeId is { } baseDamageTypeIdValue
                ? new DamageTypeId(baseDamageTypeIdValue)
                : null;

        ConditionId? imposedConditionId =
            data.ImposedConditionId is { } imposedConditionIdValue
                ? new ConditionId(imposedConditionIdValue)
                : null;

        DamageTypeId? changesUnarmedDamageTypeId =
            data.ChangesUnarmedDamageTypeId is
                { } changesUnarmedDamageTypeIdValue
                ? new DamageTypeId(changesUnarmedDamageTypeIdValue)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ElementalDisciplineDefinition(
            id: id,
            name: name,
            kiPointCost: data.KiPointCost,
            requiredMinimumLevel: data.RequiredMinimumLevel,
            grantedSpellId: grantedSpellId,
            savingThrowAbilityId: savingThrowAbilityId,
            baseDamage: baseDamage,
            baseDamageTypeId: baseDamageTypeId,
            halfDamageOnSuccessfulSave: data.HalfDamageOnSuccessfulSave,
            rangeFeet: data.RangeFeet,
            pushDistanceFeet: data.PushDistanceFeet,
            imposedConditionId: imposedConditionId,
            reachIncreaseFeet: data.ReachIncreaseFeet,
            changesUnarmedDamageTypeId: changesUnarmedDamageTypeId,
            sources: sources);
    }
}
