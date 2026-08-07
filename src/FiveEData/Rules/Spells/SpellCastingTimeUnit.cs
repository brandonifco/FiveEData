namespace FiveEData.Rules.Spells;

public enum SpellCastingTimeUnit
{
    Action,
    BonusAction,

    /// <summary>
    /// A reaction. The PHB states each reaction spell's trigger in prose
    /// ("which you take when you or a creature within 60 feet of you
    /// falls"); that clause stays in the citation, not in this data.
    /// </summary>
    Reaction,
    Minute,
    Hour
}
