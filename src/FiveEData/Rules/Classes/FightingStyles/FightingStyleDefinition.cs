using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.FightingStyles;

public sealed class FightingStyleDefinition
{
    internal FightingStyleDefinition(
        FightingStyleId id,
        string name,
        IEnumerable<ClassId> availableToClassIds,
        FightingStyleRollBonus? rollBonus,
        int? armorClassBonus,
        FightingStyleDamageDieReroll? damageDieReroll,
        FightingStyleReaction? reaction,
        bool grantsOffHandAbilityModifierDamage,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(availableToClassIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        AvailableToClassIds =
            Array.AsReadOnly(availableToClassIds.ToArray());
        RollBonus = rollBonus;
        ArmorClassBonus = armorClassBonus;
        DamageDieReroll = damageDieReroll;
        Reaction = reaction;
        GrantsOffHandAbilityModifierDamage =
            grantsOffHandAbilityModifierDamage;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public FightingStyleId Id { get; }
    public string Name { get; }
    public IReadOnlyList<ClassId> AvailableToClassIds { get; }
    public FightingStyleRollBonus? RollBonus { get; }
    public int? ArmorClassBonus { get; }
    public FightingStyleDamageDieReroll? DamageDieReroll { get; }
    public FightingStyleReaction? Reaction { get; }
    public bool GrantsOffHandAbilityModifierDamage { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
