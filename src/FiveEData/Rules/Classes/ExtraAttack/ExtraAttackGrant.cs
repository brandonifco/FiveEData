namespace FiveEData.Rules.Classes.ExtraAttack;

public readonly record struct ExtraAttackGrant
{
    public ExtraAttackGrant(int characterLevel, int attackCount)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (attackCount < 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(attackCount),
                attackCount,
                "Extra Attack grant attack count must be at least two " +
                "(one attack is the baseline every combatant already " +
                "has).");
        }

        CharacterLevel = characterLevel;
        AttackCount = attackCount;
    }

    public int CharacterLevel { get; }

    public int AttackCount { get; }
}
