using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Creatures.Skills.Serialization;

internal static class SkillDefinitionLoader
{
    public static IReadOnlyList<SkillDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<SkillDefinition> LoadFromJson(
        string json)
    {
        SkillDefinitionData[] data =
            StrictJson.DeserializeArray<SkillDefinitionData>(
                json,
                "Skill");

        var definitions =
            new List<SkillDefinition>(data.Length);
        var ids = new HashSet<SkillId>();

        for (int index = 0; index < data.Length; index++)
        {
            SkillDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid skill definition at index {index}.");
            }

            SkillDefinition definition;

            try
            {
                definition = Map(itemData);
                SkillDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid skill definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate skill ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static SkillDefinition Map(
        SkillDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new SkillId(
            data.Id
            ?? throw new ArgumentException(
                "Skill ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Skill name is required.",
                nameof(data));

        var normallyAssociatedAbilityId = new AbilityId(
            data.NormallyAssociatedAbilityId
            ?? throw new ArgumentException(
                "Skill normally associated ability ID " +
                "is required.",
                nameof(data)));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Skill sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new SkillDefinition(
            id,
            name,
            normallyAssociatedAbilityId,
            sources);
    }
}
