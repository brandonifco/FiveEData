using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.Auras.Serialization;
using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CircleForms.Serialization;
using FiveEData.Rules.Classes.CombatSuperiority;
using FiveEData.Rules.Classes.CombatSuperiority.Serialization;
using FiveEData.Rules.Classes.DiscipleOfTheElements;
using FiveEData.Rules.Classes.DiscipleOfTheElements.Serialization;
using FiveEData.Rules.Classes.DraconicResilience;
using FiveEData.Rules.Classes.HurlThroughHell;
using FiveEData.Rules.Classes.HurlThroughHell.Serialization;
using FiveEData.Rules.Classes.ImprovedCritical;
using FiveEData.Rules.Classes.ImprovedCritical.Serialization;
using FiveEData.Rules.Classes.DraconicResilience.Serialization;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.MagicalSecrets.Serialization;
using FiveEData.Rules.Classes.Portent;
using FiveEData.Rules.Classes.ShadowStep;
using FiveEData.Rules.Classes.ShadowStep.Serialization;
using FiveEData.Rules.Classes.ThunderboltStrike;
using FiveEData.Rules.Classes.ThunderboltStrike.Serialization;
using FiveEData.Rules.Classes.WrathOfTheStorm;
using FiveEData.Rules.Classes.WrathOfTheStorm.Serialization;
using FiveEData.Rules.Classes.Portent.Serialization;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Classes.DivineStrike.Serialization;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.Serialization;

internal static class SubclassDefinitionLoader
{
    public static IReadOnlyList<SubclassDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<SubclassDefinition> LoadFromJson(string json)
    {
        SubclassDefinitionData[] data =
            StrictJson.DeserializeArray<SubclassDefinitionData>(json, "Subclass");

        var subclasses = new List<SubclassDefinition>(data.Length);
        var ids = new HashSet<SubclassId>();

        for (int index = 0; index < data.Length; index++)
        {
            SubclassDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid subclass definition at index {index}.");
            }

            SubclassDefinition subclass;

            try
            {
                subclass = Map(itemData);
                SubclassDefinitionValidator.EnsureValid(subclass);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid subclass definition at {identity}.",
                    exception);
            }

            if (!ids.Add(subclass.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate subclass ID '{subclass.Id}'.");
            }

            subclasses.Add(subclass);
        }

        return subclasses;
    }

    private static SubclassDefinition Map(SubclassDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new SubclassId(
            data.Id
            ?? throw new ArgumentException(
                "Subclass ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Subclass name is required.",
                nameof(data));

        var classId = new ClassId(
            data.ClassId
            ?? throw new ArgumentException(
                "Subclass class ID is required.",
                nameof(data)));

        ClassLevelFeatureData[] levelFeatureData = data.LevelFeatures
            ?? throw new ArgumentException(
                "Subclass level features are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Subclass sources are required.",
                nameof(data));

        SpellSlotProgressionId? spellSlotProgressionId =
            data.SpellSlotProgressionId is { } spellSlotProgressionIdValue
                ? new SpellSlotProgressionId(spellSlotProgressionIdValue)
                : null;

        AbilityId? spellcastingAbilityId =
            data.SpellcastingAbilityId is { } spellcastingAbilityIdValue
                ? new AbilityId(spellcastingAbilityIdValue)
                : null;

        DivineStrikeProgressionDetail? divineStrikeProgression =
            data.DivineStrikeProgression is { } divineStrikeProgressionData
                ? DivineStrikeProgressionDetailDataMapper.Map(
                    divineStrikeProgressionData)
                : null;

        CircleFormsProgressionDetail? circleFormsProgression =
            data.CircleFormsProgression is { } circleFormsProgressionData
                ? CircleFormsProgressionDetailDataMapper.Map(
                    circleFormsProgressionData)
                : null;

        AuraOfDevotionDetail? auraOfDevotion =
            data.AuraOfDevotion is { } auraOfDevotionData
                ? AuraOfDevotionDetailDataMapper.Map(auraOfDevotionData)
                : null;

        AuraOfWardingDetail? auraOfWarding =
            data.AuraOfWarding is { } auraOfWardingData
                ? AuraOfWardingDetailDataMapper.Map(auraOfWardingData)
                : null;

        CombatSuperiorityProgressionDetail? combatSuperiorityProgression =
            data.CombatSuperiorityProgression is
                { } combatSuperiorityProgressionData
                ? CombatSuperiorityProgressionDetailDataMapper.Map(
                    combatSuperiorityProgressionData)
                : null;

        DiscipleOfTheElementsProgressionDetail?
            discipleOfTheElementsProgression =
                data.DiscipleOfTheElementsProgression is
                    { } discipleOfTheElementsProgressionData
                    ? DiscipleOfTheElementsProgressionDetailDataMapper.Map(
                        discipleOfTheElementsProgressionData)
                    : null;

        MagicalSecretsProgressionDetail? magicalSecretsProgression =
            data.MagicalSecretsProgression is
                { } magicalSecretsProgressionData
                ? MagicalSecretsProgressionDetailDataMapper.Map(
                    magicalSecretsProgressionData)
                : null;

        PortentProgressionDetail? portentProgression =
            data.PortentProgression is { } portentProgressionData
                ? PortentProgressionDetailDataMapper.Map(
                    portentProgressionData)
                : null;

        DraconicResilienceDetail? draconicResilience =
            data.DraconicResilience is { } draconicResilienceData
                ? DraconicResilienceDetailDataMapper.Map(
                    draconicResilienceData)
                : null;

        ImprovedCriticalProgressionDetail? improvedCriticalProgression =
            data.ImprovedCriticalProgression is
                { } improvedCriticalProgressionData
                ? ImprovedCriticalProgressionDetailDataMapper.Map(
                    improvedCriticalProgressionData)
                : null;

        ShadowStepDetail? shadowStep =
            data.ShadowStep is { } shadowStepData
                ? ShadowStepDetailDataMapper.Map(shadowStepData)
                : null;

        HurlThroughHellDetail? hurlThroughHell =
            data.HurlThroughHell is { } hurlThroughHellData
                ? HurlThroughHellDetailDataMapper.Map(
                    hurlThroughHellData)
                : null;

        WrathOfTheStormDetail? wrathOfTheStorm =
            data.WrathOfTheStorm is { } wrathOfTheStormData
                ? WrathOfTheStormDetailDataMapper.Map(
                    wrathOfTheStormData)
                : null;

        ThunderboltStrikeDetail? thunderboltStrike =
            data.ThunderboltStrike is { } thunderboltStrikeData
                ? ThunderboltStrikeDetailDataMapper.Map(
                    thunderboltStrikeData)
                : null;

        return new SubclassDefinition(
            id,
            name,
            classId,
            data.ChosenAtLevel,
            levelFeatureData.Select(ClassDefinitionLoader.MapLevelFeature),
            spellSlotProgressionId,
            spellcastingAbilityId,
            divineStrikeProgression,
            circleFormsProgression,
            auraOfDevotion,
            auraOfWarding,
            combatSuperiorityProgression,
            discipleOfTheElementsProgression,
            magicalSecretsProgression,
            portentProgression,
            draconicResilience,
            improvedCriticalProgression,
            shadowStep,
            hurlThroughHell,
            wrathOfTheStorm,
            thunderboltStrike,
            sourceData.Select(SourceReferenceDataMapper.Map));
    }
}
