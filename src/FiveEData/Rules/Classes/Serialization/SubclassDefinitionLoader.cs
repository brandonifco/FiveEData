using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CircleForms.Serialization;
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
            sourceData.Select(SourceReferenceDataMapper.Map));
    }
}
