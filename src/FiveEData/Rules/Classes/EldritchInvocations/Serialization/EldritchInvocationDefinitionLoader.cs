using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Classes.EldritchInvocations.Serialization;

internal static class EldritchInvocationDefinitionLoader
{
    public static IReadOnlyList<EldritchInvocationDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<EldritchInvocationDefinition> LoadFromJson(
        string json)
    {
        EldritchInvocationDefinitionData[] data =
            StrictJson.DeserializeArray<EldritchInvocationDefinitionData>(
                json,
                "Eldritch invocation");

        var definitions =
            new List<EldritchInvocationDefinition>(data.Length);
        var ids = new HashSet<EldritchInvocationId>();

        for (int index = 0; index < data.Length; index++)
        {
            EldritchInvocationDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid eldritch invocation definition at index " +
                    $"{index}.");
            }

            EldritchInvocationDefinition definition;

            try
            {
                definition = Map(itemData);
                EldritchInvocationDefinitionValidator.EnsureValid(
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
                    $"Invalid eldritch invocation definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate eldritch invocation ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static EldritchInvocationDefinition Map(
        EldritchInvocationDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new EldritchInvocationId(
            data.Id
            ?? throw new ArgumentException(
                "Eldritch invocation ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Eldritch invocation name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Eldritch invocation sources are required.",
                nameof(data));

        SpellId? grantedSpellId =
            data.GrantedSpellId is { } grantedSpellIdValue
                ? new SpellId(grantedSpellIdValue)
                : null;

        DamageTypeId? extraDamageTypeId =
            data.ExtraDamageTypeId is { } extraDamageTypeIdValue
                ? new DamageTypeId(extraDamageTypeIdValue)
                : null;

        SkillId[] skillProficiencyIds =
            (data.SkillProficiencyIds
                ?? throw new ArgumentException(
                    "Eldritch invocation skill proficiencies are " +
                    "required.",
                    nameof(data)))
            .Select(value => new SkillId(value))
            .ToArray();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new EldritchInvocationDefinition(
            id: id,
            name: name,
            requiresEldritchBlastCantrip:
                data.RequiresEldritchBlastCantrip,
            requiredMinimumLevel: data.RequiredMinimumLevel,
            requiresPactBoon: data.RequiresPactBoon,
            grantedSpellId: grantedSpellId,
            castingFrequency: data.CastingFrequency,
            waivesMaterialComponents: data.WaivesMaterialComponents,
            addsSpellcastingModifierToDamage:
                data.AddsSpellcastingModifierToDamage,
            extraDamageTypeId: extraDamageTypeId,
            skillProficiencyIds: skillProficiencyIds,
            darknessVisionRangeFeet: data.DarknessVisionRangeFeet,
            trueSightRangeFeet: data.TrueSightRangeFeet,
            eldritchBlastRangeFeet: data.EldritchBlastRangeFeet,
            eldritchBlastPushDistanceFeet:
                data.EldritchBlastPushDistanceFeet,
            canReadAllWriting: data.CanReadAllWriting,
            grantsSecondPactWeaponAttack:
                data.GrantsSecondPactWeaponAttack,
            sources: sources);
    }
}
