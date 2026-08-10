using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.CreateThrall;

public sealed record CreateThrallDetail
{
    public CreateThrallDetail(
        bool requiresIncapacitatedTarget,
        ConditionId imposedConditionId,
        bool grantsTelepathyWhileOnSamePlane)
    {
        if (string.IsNullOrWhiteSpace(imposedConditionId.Value))
        {
            throw new ArgumentException(
                "Create Thrall imposed condition ID is required.",
                nameof(imposedConditionId));
        }

        RequiresIncapacitatedTarget = requiresIncapacitatedTarget;
        ImposedConditionId = imposedConditionId;
        GrantsTelepathyWhileOnSamePlane = grantsTelepathyWhileOnSamePlane;
    }

    public bool RequiresIncapacitatedTarget { get; }

    public ConditionId ImposedConditionId { get; }

    public bool GrantsTelepathyWhileOnSamePlane { get; }
}
