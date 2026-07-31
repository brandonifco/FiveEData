namespace FiveEData.Rules.Common;

public sealed record ListedWeight
{
    public ListedWeight(Weight weight, string? qualifier = null)
    {
        if (weight.Pounds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(weight),
                weight,
                "Listed weight must be greater than zero.");
        }

        if (qualifier is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(qualifier);
        }

        Weight = weight;
        Qualifier = qualifier;
    }

    public Weight Weight { get; }
    public string? Qualifier { get; }
}
