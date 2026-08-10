using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Spells;

namespace FiveEData.Tests;

public sealed class EldritchInvocationFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new EldritchInvocationId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.eldritch-invocation.test";

        var id = new EldritchInvocationId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_ExposesAllPrerequisitesTogetherWhenPresent()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: true,
            requiredMinimumLevel: 15,
            requiresPactBoon: WarlockPactBoon.Chain,
            [CreateSource()]);

        Assert.True(definition.RequiresEldritchBlastCantrip);
        Assert.Equal(15, definition.RequiredMinimumLevel);
        Assert.Equal(WarlockPactBoon.Chain, definition.RequiresPactBoon);
    }

    [Fact]
    public void Definition_ExposesGrantedSpellAndCastingFrequencyWhenPresent()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            grantedSpellId: new SpellId("dnd5e2014.spell.mage-armor"),
            castingFrequency:
                EldritchInvocationCastingFrequency.AtWill,
            waivesMaterialComponents: true);

        Assert.Equal(
            "dnd5e2014.spell.mage-armor",
            definition.GrantedSpellId?.Value);
        Assert.Equal(
            EldritchInvocationCastingFrequency.AtWill,
            definition.CastingFrequency);
        Assert.True(definition.WaivesMaterialComponents);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        EldritchInvocationDefinition definition = Create(
            null,
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()]);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsOutOfRangeRequiredMinimumLevel()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: 21,
            requiresPactBoon: null,
            [CreateSource()]);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "between 1 and 20",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            []);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsGrantedSpellWithoutCastingFrequency()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            grantedSpellId: new SpellId("dnd5e2014.spell.mage-armor"));

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "casting frequency",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsCastingFrequencyWithoutGrantedSpell()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            castingFrequency:
                EldritchInvocationCastingFrequency.AtWill);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "casting frequency",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsWaivesMaterialComponentsWithoutGrantedSpell()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            waivesMaterialComponents: true);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "waive",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void
        Validator_RejectsExtraDamageTypeWithoutSpellcastingModifierDamage()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            extraDamageTypeId:
                new DamageTypeId("dnd5e2014.damage-type.necrotic"));

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "extra damage type",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateSkillProficiencyIds()
    {
        var skillId = new SkillId("dnd5e2014.skill.deception");

        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            skillProficiencyIds: [skillId, skillId]);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duplicates",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveDarknessVisionRangeFeet()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            darknessVisionRangeFeet: 0);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "darkness vision range",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveTrueSightRangeFeet()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            trueSightRangeFeet: 0);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "true sight range",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveEldritchBlastRangeFeet()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            eldritchBlastRangeFeet: 0);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "eldritch blast range",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveEldritchBlastPushDistanceFeet()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()],
            eldritchBlastPushDistanceFeet: 0);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "eldritch blast push distance",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new EldritchInvocationCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new EldritchInvocationCatalog(
            [
                Create(
                    "dnd5e2014.eldritch-invocation.z",
                    "Z",
                    requiresEldritchBlastCantrip: false,
                    requiredMinimumLevel: null,
                    requiresPactBoon: null,
                    [CreateSource()]),
                Create(
                    "dnd5e2014.eldritch-invocation.a",
                    "A",
                    requiresEldritchBlastCantrip: false,
                    requiredMinimumLevel: null,
                    requiresPactBoon: null,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.eldritch-invocation.a",
                "dnd5e2014.eldritch-invocation.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new EldritchInvocationId("dnd5e2014.eldritch-invocation.a");

        EldritchInvocationDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out EldritchInvocationDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new EldritchInvocationId("dnd5e2014.eldritch-invocation.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out EldritchInvocationDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new EldritchInvocationCatalog(
                [
                    Create(
                        "dnd5e2014.eldritch-invocation.duplicate",
                        "One",
                        requiresEldritchBlastCantrip: false,
                        requiredMinimumLevel: null,
                        requiresPactBoon: null,
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.eldritch-invocation.duplicate",
                        "Two",
                        requiresEldritchBlastCantrip: false,
                        requiredMinimumLevel: null,
                        requiresPactBoon: null,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new EldritchInvocationCatalog([definition]));
    }

    private static EldritchInvocationDefinition Create(
        string? id,
        string name,
        bool requiresEldritchBlastCantrip,
        int? requiredMinimumLevel,
        WarlockPactBoon? requiresPactBoon,
        IEnumerable<SourceReference> sources,
        SpellId? grantedSpellId = null,
        EldritchInvocationCastingFrequency? castingFrequency = null,
        bool waivesMaterialComponents = false,
        bool addsSpellcastingModifierToDamage = false,
        DamageTypeId? extraDamageTypeId = null,
        IEnumerable<SkillId>? skillProficiencyIds = null,
        int? darknessVisionRangeFeet = null,
        int? trueSightRangeFeet = null,
        int? eldritchBlastRangeFeet = null,
        int? eldritchBlastPushDistanceFeet = null,
        bool canReadAllWriting = false,
        bool grantsSecondPactWeaponAttack = false)
    {
        return new EldritchInvocationDefinition(
            id: id is null ? default : new EldritchInvocationId(id),
            name: name,
            requiresEldritchBlastCantrip: requiresEldritchBlastCantrip,
            requiredMinimumLevel: requiredMinimumLevel,
            requiresPactBoon: requiresPactBoon,
            grantedSpellId: grantedSpellId,
            castingFrequency: castingFrequency,
            waivesMaterialComponents: waivesMaterialComponents,
            addsSpellcastingModifierToDamage:
                addsSpellcastingModifierToDamage,
            extraDamageTypeId: extraDamageTypeId,
            skillProficiencyIds: skillProficiencyIds ?? [],
            darknessVisionRangeFeet: darknessVisionRangeFeet,
            trueSightRangeFeet: trueSightRangeFeet,
            eldritchBlastRangeFeet: eldritchBlastRangeFeet,
            eldritchBlastPushDistanceFeet: eldritchBlastPushDistanceFeet,
            canReadAllWriting: canReadAllWriting,
            grantsSecondPactWeaponAttack: grantsSecondPactWeaponAttack,
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 110);
    }
}
