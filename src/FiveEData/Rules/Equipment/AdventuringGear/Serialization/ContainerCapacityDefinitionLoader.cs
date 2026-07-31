using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.AdventuringGear.Serialization;

internal static class ContainerCapacityDefinitionLoader
{
    public static IReadOnlyList<ContainerCapacityDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ContainerCapacityDefinition> LoadFromJson(
        string json)
    {
        ContainerCapacityDefinitionData[] data =
            StrictJson.DeserializeArray<ContainerCapacityDefinitionData>(
                json,
                "Container capacities");

        var definitions =
            new List<ContainerCapacityDefinition>(data.Length);
        var gearIds = new HashSet<AdventuringGearId>();

        for (int index = 0; index < data.Length; index++)
        {
            ContainerCapacityDefinition definition;

            try
            {
                definition = Map(data[index]);
                ContainerCapacityDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(data[index].AdventuringGearId)
                    ? $"index {index}"
                    : $"'{data[index].AdventuringGearId}'";

                throw new InvalidDataException(
                    $"Invalid container-capacity definition at {identity}.",
                    exception);
            }

            if (!gearIds.Add(definition.AdventuringGearId))
            {
                throw new InvalidDataException(
                    $"Duplicate container-capacity adventuring gear ID '{definition.AdventuringGearId}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ContainerCapacityDefinition Map(
        ContainerCapacityDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var gearId = new AdventuringGearId(
            data.AdventuringGearId
            ?? throw new ArgumentException(
                "Container-capacity adventuring gear ID is required.",
                nameof(data)));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Container-capacity sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ContainerCapacityDefinition(
            gearId,
            MapVolume(data.SolidVolume),
            MapVolume(data.LiquidVolume),
            data.GearWeightCapacityPounds is { } pounds
                ? new Weight(pounds)
                : null,
            data.AllowsExteriorItemAttachment,
            sources);
    }

    private static ContainerVolume? MapVolume(ContainerVolumeData? data)
    {
        return data is null
            ? null
            : new ContainerVolume(data.Amount, data.Unit);
    }
}
