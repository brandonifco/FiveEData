using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.Auras.Serialization;
using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Classes.BardicInspiration.Serialization;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.ChannelDivinity.Serialization;
using FiveEData.Rules.Classes.EldritchInvocationsKnown;
using FiveEData.Rules.Classes.EldritchInvocationsKnown.Serialization;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.FontOfMagic;
using FiveEData.Rules.Classes.FontOfMagic.Serialization;
using FiveEData.Rules.Classes.Ki;
using FiveEData.Rules.Classes.Ki.Serialization;
using FiveEData.Rules.Classes.MartialArts;
using FiveEData.Rules.Classes.MartialArts.Serialization;
using FiveEData.Rules.Classes.MysticArcanum;
using FiveEData.Rules.Classes.MysticArcanum.Serialization;
using FiveEData.Rules.Classes.Rage;
using FiveEData.Rules.Classes.Rage.Serialization;
using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Classes.SneakAttack.Serialization;
using FiveEData.Rules.Classes.SongOfRest;
using FiveEData.Rules.Classes.SongOfRest.Serialization;
using FiveEData.Rules.Classes.SorceryPoints;
using FiveEData.Rules.Classes.SorceryPoints.Serialization;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.UnarmoredMovement;
using FiveEData.Rules.Classes.UnarmoredMovement.Serialization;
using FiveEData.Rules.Classes.WildShape;
using FiveEData.Rules.Classes.WildShape.Serialization;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes.Serialization;

internal static class ClassDefinitionLoader
{
    public static IReadOnlyList<ClassDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ClassDefinition> LoadFromJson(string json)
    {
        ClassDefinitionData[] data =
            StrictJson.DeserializeArray<ClassDefinitionData>(json, "Class");

        var classes = new List<ClassDefinition>(data.Length);
        var ids = new HashSet<ClassId>();

        for (int index = 0; index < data.Length; index++)
        {
            ClassDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid class definition at index {index}.");
            }

            ClassDefinition @class;

            try
            {
                @class = Map(itemData);
                ClassDefinitionValidator.EnsureValid(@class);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid class definition at {identity}.",
                    exception);
            }

            if (!ids.Add(@class.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate class ID '{@class.Id}'.");
            }

            classes.Add(@class);
        }

        return classes;
    }

    private static ClassDefinition Map(ClassDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ClassId(
            data.Id
            ?? throw new ArgumentException(
                "Class ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Class name is required.",
                nameof(data));

        string[] primaryAbilityIdValues = data.PrimaryAbilityIds
            ?? throw new ArgumentException(
                "Class primary ability IDs are required.",
                nameof(data));

        string[] savingThrowIdValues = data.SavingThrowProficiencyIds
            ?? throw new ArgumentException(
                "Class saving throw proficiency IDs are required.",
                nameof(data));

        ArmorCategory[] armorProficiencyCategories =
            data.ArmorProficiencyCategories
            ?? throw new ArgumentException(
                "Class armor proficiency categories are required.",
                nameof(data));

        WeaponProficiencyCategory[] weaponProficiencyCategories =
            data.WeaponProficiencyCategories
            ?? throw new ArgumentException(
                "Class weapon proficiency categories are required.",
                nameof(data));

        string[] weaponProficiencyIdValues = data.WeaponProficiencyIds
            ?? throw new ArgumentException(
                "Class weapon proficiency IDs are required.",
                nameof(data));

        string[] skillChoiceOptionIdValues = data.SkillChoiceOptionIds
            ?? throw new ArgumentException(
                "Class skill choice option IDs are required.",
                nameof(data));

        ClassLevelFeatureData[] levelFeatureData = data.LevelFeatures
            ?? throw new ArgumentException(
                "Class level features are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Class sources are required.",
                nameof(data));

        SpellSlotProgressionId? spellSlotProgressionId =
            data.SpellSlotProgressionId is { } spellSlotProgressionIdValue
                ? new SpellSlotProgressionId(spellSlotProgressionIdValue)
                : null;

        AbilityId? spellcastingAbilityId =
            data.SpellcastingAbilityId is { } spellcastingAbilityIdValue
                ? new AbilityId(spellcastingAbilityIdValue)
                : null;

        ExtraAttackProgressionId? extraAttackProgressionId =
            data.ExtraAttackProgressionId is
                { } extraAttackProgressionIdValue
                ? new ExtraAttackProgressionId(
                    extraAttackProgressionIdValue)
                : null;

        RageProgressionDetail? rageProgression =
            data.RageProgression is { } rageProgressionData
                ? RageProgressionDetailDataMapper.Map(rageProgressionData)
                : null;

        SneakAttackProgressionDetail? sneakAttackProgression =
            data.SneakAttackProgression is { } sneakAttackProgressionData
                ? SneakAttackProgressionDetailDataMapper.Map(
                    sneakAttackProgressionData)
                : null;

        KiProgressionDetail? kiProgression =
            data.KiProgression is { } kiProgressionData
                ? KiProgressionDetailDataMapper.Map(kiProgressionData)
                : null;

        MartialArtsProgressionDetail? martialArtsProgression =
            data.MartialArtsProgression is { } martialArtsProgressionData
                ? MartialArtsProgressionDetailDataMapper.Map(
                    martialArtsProgressionData)
                : null;

        UnarmoredMovementProgressionDetail? unarmoredMovementProgression =
            data.UnarmoredMovementProgression is
                { } unarmoredMovementProgressionData
                ? UnarmoredMovementProgressionDetailDataMapper.Map(
                    unarmoredMovementProgressionData)
                : null;

        SorceryPointsProgressionDetail? sorceryPointsProgression =
            data.SorceryPointsProgression is { } sorceryPointsProgressionData
                ? SorceryPointsProgressionDetailDataMapper.Map(
                    sorceryPointsProgressionData)
                : null;

        WildShapeProgressionDetail? wildShapeProgression =
            data.WildShapeProgression is { } wildShapeProgressionData
                ? WildShapeProgressionDetailDataMapper.Map(
                    wildShapeProgressionData)
                : null;

        AuraOfProtectionDetail? auraOfProtection =
            data.AuraOfProtection is { } auraOfProtectionData
                ? AuraOfProtectionDetailDataMapper.Map(auraOfProtectionData)
                : null;

        AuraOfCourageDetail? auraOfCourage =
            data.AuraOfCourage is { } auraOfCourageData
                ? AuraOfCourageDetailDataMapper.Map(auraOfCourageData)
                : null;

        BardicInspirationProgressionDetail? bardicInspirationProgression =
            data.BardicInspirationProgression is
                { } bardicInspirationProgressionData
                ? BardicInspirationProgressionDetailDataMapper.Map(
                    bardicInspirationProgressionData)
                : null;

        ChannelDivinityProgressionDetail? channelDivinityProgression =
            data.ChannelDivinityProgression is
                { } channelDivinityProgressionData
                ? ChannelDivinityProgressionDetailDataMapper.Map(
                    channelDivinityProgressionData)
                : null;

        MysticArcanumProgressionDetail? mysticArcanumProgression =
            data.MysticArcanumProgression is
                { } mysticArcanumProgressionData
                ? MysticArcanumProgressionDetailDataMapper.Map(
                    mysticArcanumProgressionData)
                : null;

        FontOfMagicConversionDetail? fontOfMagicConversion =
            data.FontOfMagicConversion is { } fontOfMagicConversionData
                ? FontOfMagicConversionDetailDataMapper.Map(
                    fontOfMagicConversionData)
                : null;

        SongOfRestProgressionDetail? songOfRestProgression =
            data.SongOfRestProgression is { } songOfRestProgressionData
                ? SongOfRestProgressionDetailDataMapper.Map(
                    songOfRestProgressionData)
                : null;

        EldritchInvocationsKnownProgressionDetail?
            eldritchInvocationsKnownProgression =
                data.EldritchInvocationsKnownProgression is
                    { } eldritchInvocationsKnownProgressionData
                    ? EldritchInvocationsKnownProgressionDetailDataMapper.Map(
                        eldritchInvocationsKnownProgressionData)
                    : null;

        return new ClassDefinition(
            id,
            name,
            new DiceExpression(1, data.HitDieSides),
            primaryAbilityIdValues.Select(value => new AbilityId(value)),
            data.RequiresAllPrimaryAbilities,
            savingThrowIdValues.Select(value => new AbilityId(value)),
            armorProficiencyCategories,
            data.ProficientWithShields,
            weaponProficiencyCategories,
            weaponProficiencyIdValues.Select(value => new WeaponId(value)),
            data.SkillChoiceCount,
            skillChoiceOptionIdValues.Select(value => new SkillId(value)),
            levelFeatureData.Select(MapLevelFeature),
            spellSlotProgressionId,
            spellcastingAbilityId,
            extraAttackProgressionId,
            rageProgression,
            sneakAttackProgression,
            kiProgression,
            martialArtsProgression,
            unarmoredMovementProgression,
            sorceryPointsProgression,
            wildShapeProgression,
            auraOfProtection,
            auraOfCourage,
            bardicInspirationProgression,
            channelDivinityProgression,
            mysticArcanumProgression,
            fontOfMagicConversion,
            songOfRestProgression,
            eldritchInvocationsKnownProgression,
            sourceData.Select(SourceReferenceDataMapper.Map));
    }

    internal static ClassLevelFeature MapLevelFeature(ClassLevelFeatureData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var featureRuleId = new RuleId(
            data.FeatureRuleId
            ?? throw new ArgumentException(
                "Class level feature rule ID is required.",
                nameof(data)));

        return new ClassLevelFeature(data.Level, featureRuleId);
    }
}
