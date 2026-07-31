namespace FiveEData.Rules.Common;

public readonly record struct Money
{
    public Money(long copperPieces)
    {
        if (copperPieces < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(copperPieces),
                copperPieces,
                "Money cannot have a negative value.");
        }

        CopperPieces = copperPieces;
    }

    public long CopperPieces { get; }

    public static Money FromCopper(long amount) => FromUnits(amount, 1);

    public static Money FromSilver(long amount) => FromUnits(amount, 10);

    public static Money FromElectrum(long amount) => FromUnits(amount, 50);

    public static Money FromGold(long amount) => FromUnits(amount, 100);

    public static Money FromPlatinum(long amount) => FromUnits(amount, 1_000);

    private static Money FromUnits(long amount, long copperPiecesPerUnit)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Money cannot have a negative value.");
        }

        return new Money(checked(amount * copperPiecesPerUnit));
    }
}
