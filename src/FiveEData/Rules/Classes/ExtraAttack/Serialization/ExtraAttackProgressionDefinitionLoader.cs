using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.ExtraAttack.Serialization;

internal static class ExtraAttackProgressionDefinitionLoader
{
    public static IReadOnlyList<ExtraAttackProgressionDefinition>
        LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ExtraAttackProgressionDefinition>
        LoadFromJson(string json)
    {
        ExtraAttackProgressionDefinitionData[] data =
            StrictJson.DeserializeArray<ExtraAttackProgressionDefinitionData>(
                json,
                "Extra Attack progression");

        var definitions =
            new List<ExtraAttackProgressionDefinition>(data.Length);
        var ids = new HashSet<ExtraAttackProgressionId>();

        for (int index = 0; index < data.Length; index++)
        {
            ExtraAttackProgressionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    "Invalid Extra Attack progression definition at " +
                    $"index {index}.");
            }

            ExtraAttackProgressionDefinition definition;

            try
            {
                definition = Map(itemData);
                ExtraAttackProgressionDefinitionValidator.EnsureValid(
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
                    "Invalid Extra Attack progression definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    "Duplicate Extra Attack progression ID " +
                    $"'{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ExtraAttackProgressionDefinition Map(
        ExtraAttackProgressionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ExtraAttackProgressionId(
            data.Id
            ?? throw new ArgumentException(
                "Extra Attack progression ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Extra Attack progression name is required.",
                nameof(data));

        ExtraAttackGrantData[] grantData =
            data.Grants
            ?? throw new ArgumentException(
                "Extra Attack progression grants are required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Extra Attack progression sources are required.",
                nameof(data));

        ExtraAttackGrant[] grants = grantData
            .Select(
                grant => new ExtraAttackGrant(
                    grant.CharacterLevel,
                    grant.AttackCount))
            .ToArray();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ExtraAttackProgressionDefinition(
            id,
            name,
            grants,
            sources);
    }
}
