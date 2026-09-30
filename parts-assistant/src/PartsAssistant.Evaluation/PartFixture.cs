using PartsAssistant.Domain.Enums;

namespace PartsAssistant.Evaluation;

public sealed record PartFixture(
    string PartNumber,
    string Description,
    string Brand,
    PartCategory Category,
    decimal TradePriceGbp,
    decimal RetailPriceGbp,
    int StockQuantity,
    bool IsDiscontinued,
    string? SupersededByPartNumber,
    IReadOnlyList<string> FitsVehicleKeys);