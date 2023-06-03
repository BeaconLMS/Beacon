namespace Beacon.Common.Laboratories.Events;

public sealed record LaboratoryCreatedEvent(LaboratoryDto Laboratory) : IBeaconEvent<LaboratoryCreatedEvent>
{
    public static Action<LaboratoryCreatedEvent>? OnTrigger { get; set; }
}
