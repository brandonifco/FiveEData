namespace FiveEData.Rules.Spells;

public readonly record struct SpellCastingTime
{
    public SpellCastingTime(int amount, SpellCastingTimeUnit unit)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Spell casting time amount must be greater than zero.");
        }

        if (!Enum.IsDefined(unit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(unit),
                unit,
                "Spell casting time unit must be a defined value.");
        }

        Amount = amount;
        Unit = unit;
    }

    public int Amount { get; }
    public SpellCastingTimeUnit Unit { get; }
}
