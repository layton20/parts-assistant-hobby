using PartsAssistant.Domain;
using PartsAssistant.Domain.Enums;

namespace Domain.Tests;

public sealed class PartsTest
{
    [Fact]
    public void Create_WithoutSupersedingPart_HasNoSupersededByPartId()
    {
        var part = new Part
        {
            Id = Guid.NewGuid(),
            PartNumber = "BP-1042-F",
            Description = "Front brake pad set",
            Brand = "Vantex",
            Category = PartCategory.Brakes,
            TradePriceGbp = 28.40m,
            RetailPriceGbp = 38.34m,
            StockQuantity = 14,
            IsDiscontinued = false
        };

        Assert.Null(part.SupersededByPartId);
    }
}