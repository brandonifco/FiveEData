using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Ammunition;

namespace FiveEData.Rules.Equipment.Weapons.Serialization;

internal static class WeaponDefinitionLoader
{
    public static IReadOnlyList<WeaponDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<WeaponDefinition> LoadFromJson(string json)
    {
        WeaponDefinitionData[] data =
            StrictJson.DeserializeArray<WeaponDefinitionData>(
                json,
                "Weapon");

        var weapons = new List<WeaponDefinition>(data.Length);
        var ids = new HashSet<WeaponId>();

        for (int index = 0; index < data.Length; index++)
        {
            WeaponDefinitionData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid weapon definition at index {index}.");
            }

            WeaponDefinition weapon;

            try
            {
                weapon = Map(itemData);
                WeaponDefinitionValidator.EnsureValid(weapon);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid weapon definition at {identity}.",
                    exception);
            }

            if (!ids.Add(weapon.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate weapon ID '{weapon.Id}'.");
            }

            weapons.Add(weapon);
        }

        return weapons;
    }

    private static WeaponDefinition Map(WeaponDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new WeaponId(
            data.Id
            ?? throw new ArgumentException(
                "Weapon ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Weapon name is required.",
                nameof(data));

        WeaponProperty[] properties = data.Properties
            ?? throw new ArgumentException(
                "Weapon properties are required.",
                nameof(data));

        string[] specialRuleIdValues = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Weapon special-rule IDs are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Weapon sources are required.",
                nameof(data));

        WeaponDamage? damage = data.Damage is null
            ? null
            : MapDamage(data.Damage);

        WeaponRange? range = data.Range is null
            ? null
            : new WeaponRange(
                new Distance(data.Range.NormalFeet),
                new Distance(data.Range.LongFeet));

        DiceExpression? versatileDamage = data.VersatileDamage is null
            ? null
            : MapDice(data.VersatileDamage);

        AmmunitionTypeId? ammunitionTypeId =
            string.IsNullOrWhiteSpace(data.AmmunitionTypeId)
                ? null
                : new AmmunitionTypeId(data.AmmunitionTypeId);

        HashSet<RuleId> specialRuleIds = specialRuleIdValues
            .Select(value => new RuleId(value))
            .ToHashSet();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new WeaponDefinition(
            id,
            name,
            data.ProficiencyCategory,
            data.UsageCategory,
            data.Cost is null
                ? null
                : new Money(data.Cost.CopperPieces),
            data.Weight is null
                ? null
                : new Weight(data.Weight.Pounds),
            damage,
            properties,
            range,
            versatileDamage,
            ammunitionTypeId,
            specialRuleIds,
            sources);
    }

    private static WeaponDamage MapDamage(WeaponDamageData data)
    {
        DiceExpression? dice = data.Dice is null
            ? null
            : MapDice(data.Dice);

        return new WeaponDamage(
            dice,
            data.FixedAmount,
            data.Type);
    }

    private static DiceExpression MapDice(DiceExpressionData data)
    {
        return new DiceExpression(
            data.Count,
            data.Sides);
    }
}
