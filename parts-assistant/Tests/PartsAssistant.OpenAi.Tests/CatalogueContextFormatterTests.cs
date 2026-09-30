using PartsAssistant.Domain;
using PartsAssistant.Domain.Enums;

namespace PartsAssistant.OpenAi.Tests;

public sealed class CatalogueContextFormatterTests
{
    [Fact]
    public void Format_IncludesTradePriceRetailPriceAndStock()
    {
        var part = CreatePart("BP-1042-F", tradePriceGbp: 28.40m, retailPriceGbp: 38.34m, stockQuantity: 14);

        var text = CatalogueContextFormatter.Format([part], [], []);

        Assert.Contains("trade £28.40", text);
        Assert.Contains("retail £38.34", text);
        Assert.Contains("stock 14", text);
    }

    [Fact]
    public void Format_DiscontinuedPartWithSuccessor_NamesTheReplacementPartNumber()
    {
        var currentPart = CreatePart("FL-3002");
        var oldPart = CreatePart("FL-3000", isDiscontinued: true, supersededByPartId: currentPart.Id);

        var text = CatalogueContextFormatter.Format([oldPart, currentPart], [], []);

        Assert.Contains("discontinued, superseded by FL-3002", text);
    }

    [Fact]
    public void Format_DiscontinuedPartWithNoSuccessor_SaysNoReplacementOnRecord()
    {
        var part = CreatePart("LT-6100", isDiscontinued: true);

        var text = CatalogueContextFormatter.Format([part], [], []);

        Assert.Contains("discontinued, no replacement on record", text);
    }

    [Fact]
    public void Format_ListsFittingVehiclesSortedAlphabetically()
    {
        var part = CreatePart("BP-1042-F");
        var stratus = CreateVehicle("Marlow", "Stratus");
        var corvid = CreateVehicle("Halden", "Corvid");
        var fitments = new[]
        {
            new PartFitment { PartId = part.Id, VehicleId = stratus.Id },
            new PartFitment { PartId = part.Id, VehicleId = corvid.Id }
        };

        var text = CatalogueContextFormatter.Format([part], [stratus, corvid], fitments);

        var corvidIndex = text.IndexOf("Halden Corvid", StringComparison.Ordinal);
        var stratusIndex = text.IndexOf("Marlow Stratus", StringComparison.Ordinal);
        Assert.True(corvidIndex >= 0 && corvidIndex < stratusIndex);
    }

    [Fact]
    public void Format_OrdersPartsByPartNumber()
    {
        var laterPart = CreatePart("FL-3002");
        var earlierPart = CreatePart("BP-1042-F");

        var text = CatalogueContextFormatter.Format([laterPart, earlierPart], [], []);

        Assert.True(text.IndexOf("BP-1042-F", StringComparison.Ordinal) < text.IndexOf("FL-3002", StringComparison.Ordinal));
    }

    private static Part CreatePart(
        string partNumber,
        decimal tradePriceGbp = 10m,
        decimal retailPriceGbp = 13.50m,
        int stockQuantity = 1,
        bool isDiscontinued = false,
        Guid? supersededByPartId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            PartNumber = partNumber,
            Description = "Test part",
            Brand = "Vantex",
            Category = PartCategory.Brakes,
            TradePriceGbp = tradePriceGbp,
            RetailPriceGbp = retailPriceGbp,
            StockQuantity = stockQuantity,
            IsDiscontinued = isDiscontinued,
            SupersededByPartId = supersededByPartId
        };

    private static Vehicle CreateVehicle(string make, string model) =>
        new()
        {
            Id = Guid.NewGuid(),
            Make = make,
            Model = model,
            Engine = "1.6 Petrol",
            YearFrom = 2015,
            YearTo = 2019
        };
}
