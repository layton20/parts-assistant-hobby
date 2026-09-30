namespace PartsAssistant.Domain;

public sealed class PartFitment
{
    public required Guid PartId { get; init; }
    public required Guid VehicleId { get; init; }
}