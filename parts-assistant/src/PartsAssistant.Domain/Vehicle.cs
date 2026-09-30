namespace PartsAssistant.Domain;

public sealed class Vehicle
{
    public required Guid Id { get; init; }
    public required string Make { get; init; }
    public required string Model { get; init; }
    public required string Engine { get; init; }
    public required int YearFrom { get; init; }
    public required int YearTo { get; init; }

    public bool IsProducedIn(int year) => year >= YearFrom && year <= YearTo;
}