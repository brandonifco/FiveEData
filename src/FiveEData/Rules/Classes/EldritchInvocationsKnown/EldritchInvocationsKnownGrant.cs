namespace FiveEData.Rules.Classes.EldritchInvocationsKnown;

public readonly record struct EldritchInvocationsKnownGrant
{
    public EldritchInvocationsKnownGrant(
        int characterLevel,
        int invocationsKnown)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (invocationsKnown <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(invocationsKnown),
                invocationsKnown,
                "Invocations known must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        InvocationsKnown = invocationsKnown;
    }

    public int CharacterLevel { get; }

    public int InvocationsKnown { get; }
}
