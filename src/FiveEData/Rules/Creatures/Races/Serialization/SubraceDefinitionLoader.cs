using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Races.Serialization;

internal static class SubraceDefinitionLoader
{
    public static IReadOnlyList<SubraceDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<SubraceDefinition> LoadFromJson(string json)
    {
        SubraceDefinitionData[] data =
            StrictJson.DeserializeArray<SubraceDefinitionData>(json, "Subrace");

        var subraces = new List<SubraceDefinition>(data.Length);
        var ids = new HashSet<SubraceId>();

        for (int index = 0; index < data.Length; index++)
        {
            SubraceDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid subrace definition at index {index}.");
            }

            SubraceDefinition subrace;

            try
            {
                subrace = Map(itemData);
                SubraceDefinitionValidator.EnsureValid(subrace);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid subrace definition at {identity}.",
                    exception);
            }

            if (!ids.Add(subrace.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate subrace ID '{subrace.Id}'.");
            }

            subraces.Add(subrace);
        }

        return subraces;
    }

    private static SubraceDefinition Map(SubraceDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new SubraceId(
            data.Id
            ?? throw new ArgumentException(
                "Subrace ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Subrace name is required.",
                nameof(data));

        var raceId = new RaceId(
            data.RaceId
            ?? throw new ArgumentException(
                "Subrace race ID is required.",
                nameof(data)));

        RaceAbilityScoreIncreaseData[] abilityScoreIncreaseData =
            data.AbilityScoreIncreases
            ?? throw new ArgumentException(
                "Subrace ability score increases are required.",
                nameof(data));

        string[] traitRuleIdValues = data.TraitRuleIds
            ?? throw new ArgumentException(
                "Subrace trait rule IDs are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Subrace sources are required.",
                nameof(data));

        RaceAbilityScoreIncrease[] abilityScoreIncreases =
            abilityScoreIncreaseData
                .Select(RaceDefinitionLoader.MapAbilityScoreIncrease)
                .ToArray();

        RuleId[] traitRuleIds = traitRuleIdValues
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        Distance? speed = data.SpeedFeet is { } speedFeet
            ? new Distance(speedFeet)
            : null;

        return new SubraceDefinition(
            id,
            name,
            raceId,
            abilityScoreIncreases,
            speed,
            data.AdditionalLanguageChoiceCount,
            traitRuleIds,
            sources);
    }
}
