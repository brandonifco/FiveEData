namespace FiveEData.Rules.Equipment;

public readonly record struct EquipmentChangeTiming
{
    public EquipmentChangeTiming(
        EquipmentChangeDuration don,
        EquipmentChangeDuration doff)
    {
        Don = don;
        Doff = doff;
    }

    public EquipmentChangeDuration Don { get; }
    public EquipmentChangeDuration Doff { get; }
}
