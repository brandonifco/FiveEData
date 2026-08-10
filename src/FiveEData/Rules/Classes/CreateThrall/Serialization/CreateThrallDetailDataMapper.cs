using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.CreateThrall.Serialization;

internal static class CreateThrallDetailDataMapper
{
    public static CreateThrallDetail Map(CreateThrallDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string imposedConditionIdValue = data.ImposedConditionId
            ?? throw new ArgumentException(
                "Create Thrall imposed condition ID is required.",
                nameof(data));

        return new CreateThrallDetail(
            data.RequiresIncapacitatedTarget,
            new ConditionId(imposedConditionIdValue),
            data.GrantsTelepathyWhileOnSamePlane);
    }
}
