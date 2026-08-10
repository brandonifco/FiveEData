using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Classes.TotemWarriorOptions;

public sealed class TotemWarriorOptionDefinition
{
    internal TotemWarriorOptionDefinition(
        TotemWarriorOptionId id,
        string name,
        int requiredLevel,
        bool requiresRaging,
        bool requiresNotWearingHeavyArmor,
        DamageTypeId? resistsAllDamageExceptTypeId,
        bool imposesDisadvantageOnOpportunityAttacksAgainstYou,
        bool grantsDashAsBonusAction,
        int? grantsAlliesAdvantageOnMeleeAttacksWithinFeet,
        bool doublesCarryingCapacity,
        bool grantsAdvantageOnStrengthChecksToMoveObjects,
        int? clearSightRangeFeet,
        int? clearSightDetailEquivalentRangeFeet,
        bool ignoresDimLightPerceptionDisadvantage,
        TravelPaceId? tracksAtTravelPaceId,
        TravelPaceId? movesStealthilyAtTravelPaceId,
        int? imposesDisadvantageOnAttacksAgainstOthersWithinFeet,
        bool grantsFlyingSpeedEqualToWalkingSpeed,
        ConditionId? imposedConditionId,
        CreatureSizeId? maximumTargetSizeId,
        bool imposedConditionRequiresBonusAction,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RequiredLevel = requiredLevel;
        RequiresRaging = requiresRaging;
        RequiresNotWearingHeavyArmor = requiresNotWearingHeavyArmor;
        ResistsAllDamageExceptTypeId = resistsAllDamageExceptTypeId;
        ImposesDisadvantageOnOpportunityAttacksAgainstYou =
            imposesDisadvantageOnOpportunityAttacksAgainstYou;
        GrantsDashAsBonusAction = grantsDashAsBonusAction;
        GrantsAlliesAdvantageOnMeleeAttacksWithinFeet =
            grantsAlliesAdvantageOnMeleeAttacksWithinFeet;
        DoublesCarryingCapacity = doublesCarryingCapacity;
        GrantsAdvantageOnStrengthChecksToMoveObjects =
            grantsAdvantageOnStrengthChecksToMoveObjects;
        ClearSightRangeFeet = clearSightRangeFeet;
        ClearSightDetailEquivalentRangeFeet =
            clearSightDetailEquivalentRangeFeet;
        IgnoresDimLightPerceptionDisadvantage =
            ignoresDimLightPerceptionDisadvantage;
        TracksAtTravelPaceId = tracksAtTravelPaceId;
        MovesStealthilyAtTravelPaceId = movesStealthilyAtTravelPaceId;
        ImposesDisadvantageOnAttacksAgainstOthersWithinFeet =
            imposesDisadvantageOnAttacksAgainstOthersWithinFeet;
        GrantsFlyingSpeedEqualToWalkingSpeed =
            grantsFlyingSpeedEqualToWalkingSpeed;
        ImposedConditionId = imposedConditionId;
        MaximumTargetSizeId = maximumTargetSizeId;
        ImposedConditionRequiresBonusAction =
            imposedConditionRequiresBonusAction;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public TotemWarriorOptionId Id { get; }
    public string Name { get; }
    public int RequiredLevel { get; }
    public bool RequiresRaging { get; }
    public bool RequiresNotWearingHeavyArmor { get; }
    public DamageTypeId? ResistsAllDamageExceptTypeId { get; }
    public bool ImposesDisadvantageOnOpportunityAttacksAgainstYou { get; }
    public bool GrantsDashAsBonusAction { get; }
    public int? GrantsAlliesAdvantageOnMeleeAttacksWithinFeet { get; }
    public bool DoublesCarryingCapacity { get; }
    public bool GrantsAdvantageOnStrengthChecksToMoveObjects { get; }
    public int? ClearSightRangeFeet { get; }
    public int? ClearSightDetailEquivalentRangeFeet { get; }
    public bool IgnoresDimLightPerceptionDisadvantage { get; }
    public TravelPaceId? TracksAtTravelPaceId { get; }
    public TravelPaceId? MovesStealthilyAtTravelPaceId { get; }
    public int? ImposesDisadvantageOnAttacksAgainstOthersWithinFeet { get; }
    public bool GrantsFlyingSpeedEqualToWalkingSpeed { get; }
    public ConditionId? ImposedConditionId { get; }
    public CreatureSizeId? MaximumTargetSizeId { get; }
    public bool ImposedConditionRequiresBonusAction { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
