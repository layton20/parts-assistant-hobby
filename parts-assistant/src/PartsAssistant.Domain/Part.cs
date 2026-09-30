using PartsAssistant.Domain.Enums;

namespace PartsAssistant.Domain;

public sealed class Part
{
    public required Guid Id { get; init; }
    public required string PartNumber { get; init; }
    public required string Description { get; init; }
    public required string Brand { get; init; }
    public required PartCategory Category { get; init; }
    public required decimal TradePriceGbp { get; init; }
    public required decimal RetailPriceGbp { get; init; }
    public required int StockQuantity { get; init; }
    public required bool IsDiscontinued { get; init; }
    public Guid? SupersededByPartId { get; init; }
}