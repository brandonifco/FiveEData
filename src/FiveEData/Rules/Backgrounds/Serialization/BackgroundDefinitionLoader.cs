using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Backgrounds.Serialization;

internal static class BackgroundDefinitionLoader
{
    public static IReadOnlyList<BackgroundDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<BackgroundDefinition> LoadFromJson(string json)
    {
        BackgroundDefinitionData[] data =
            StrictJson.DeserializeArray<BackgroundDefinitionData>(
                json,
                "Background");

        var backgrounds = new List<BackgroundDefinition>(data.Length);
        var ids = new HashSet<BackgroundId>();

        for (int index = 0; index < data.Length; index++)
        {
            BackgroundDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid background definition at index {index}.");
            }

            BackgroundDefinition background;

            try
            {
                background = Map(itemData);
                BackgroundDefinitionValidator.EnsureValid(background);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid background definition at {identity}.",
                    exception);
            }

            if (!ids.Add(background.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate background ID '{background.Id}'.");
            }

            backgrounds.Add(background);
        }

        return backgrounds;
    }

    private static BackgroundDefinition Map(BackgroundDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new BackgroundId(
            data.Id
            ?? throw new ArgumentException(
                "Background ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Background name is required.",
                nameof(data));

        string[] skillProficiencyIdValues = data.SkillProficiencyIds
            ?? throw new ArgumentException(
                "Background skill proficiency IDs are required.",
                nameof(data));

        var featureRuleId = new RuleId(
            data.FeatureRuleId
            ?? throw new ArgumentException(
                "Background feature rule ID is required.",
                nameof(data)));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Background sources are required.",
                nameof(data));

        SkillId[] skillProficiencyIds = skillProficiencyIdValues
            .Select(value => new SkillId(value))
            .ToArray();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        LifestyleId? sustainedLifestyleId =
            data.SustainedLifestyleId is { } sustainedLifestyleIdValue
                ? new LifestyleId(sustainedLifestyleIdValue)
                : null;

        string[] toolProficiencyIdValues = data.ToolProficiencyIds
            ?? throw new ArgumentException(
                "Background tool proficiency IDs are required.",
                nameof(data));

        ToolProficiencyChoice? toolProficiencyChoice =
            data.ToolProficiencyChoice is { } toolProficiencyChoiceData
                ? ToolProficiencyChoiceDataMapper.Map(
                    toolProficiencyChoiceData)
                : null;

        VehicleKind[] vehicleProficiencyKinds = data.VehicleProficiencyKinds
            ?? throw new ArgumentException(
                "Background vehicle proficiency kinds are required.",
                nameof(data));

        return new BackgroundDefinition(
            id,
            name,
            skillProficiencyIds,
            data.LanguageChoiceCount,
            featureRuleId,
            sustainedLifestyleId,
            data.AdditionalPeopleFedPerDay,
            data.GuildDuesGoldPerMonth,
            data.FastTravelSpeedMultiplier,
            toolProficiencyIdValues.Select(value => new ToolId(value)),
            toolProficiencyChoice,
            vehicleProficiencyKinds,
            sources);
    }
}
