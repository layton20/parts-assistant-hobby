using PartsAssistant.Domain;
using PartsAssistant.Domain.Enums;

namespace Domain.Tests;

public sealed class InMemoryPartCatalogueTests
{
    [Fact]
    public void FindByPartNumber_IgnoresCaseAndSurroundingWhitespace()
    {
        var catalogue = new InMemoryPartCatalogue([CreatePart("BP-1042-F")]);

        var foundPart = catalogue.FindByPartNumber("  bp-1042-f ");

        Assert.Equal("BP-1042-F", foundPart?.PartNumber);
    }

    [Fact]
    public void FindByPartNumber_WithUnknownPartNumber_ReturnsNull()
    {
        var catalogue = new InMemoryPartCatalogue([CreatePart("BP-1042-F")]);

        Assert.Null(catalogue.FindByPartNumber("BP-9999-F"));
    }

    [Fact]
    public void GetSupersessionChain_FollowsEveryReplacementToTheCurrentPart()
    {
        var currentPart = CreatePart("FL-3002");
        var middlePart = CreatePart("FL-3001", supersededByPartId: currentPart.Id);
        var oldestPart = CreatePart("FL-3000", supersededByPartId: middlePart.Id);
        var catalogue = new InMemoryPartCatalogue([oldestPart, middlePart, currentPart]);

        var chain = catalogue.GetSupersessionChain("FL-3000");

        Assert.Equal(new[] { "FL-3000", "FL-3001", "FL-3002" }, chain.Select(part => part.PartNumber));
    }

    [Fact]
    public void GetSupersessionChain_ForPartWithNoReplacement_ReturnsOnlyThatPart()
    {
        var catalogue = new InMemoryPartCatalogue([CreatePart("FL-3002")]);

        var chain = catalogue.GetSupersessionChain("FL-3002");

        Assert.Single(chain);
    }

    [Fact]
    public void GetSupersessionChain_WhenPartsSupersedeEachOther_Throws()
    {
        var firstPartId = Guid.NewGuid();
        var secondPartId = Guid.NewGuid();
        var firstPart = CreatePart("FL-1", id: firstPartId, supersededByPartId: secondPartId);
        var secondPart = CreatePart("FL-2", id: secondPartId, supersededByPartId: firstPartId);
        var catalogue = new InMemoryPartCatalogue([firstPart, secondPart]);

        Assert.Throws<InvalidOperationException>(() => catalogue.GetSupersessionChain("FL-1"));
    }

    [Fact]
    public void GetSupersessionChain_WhenSupersedingPartIsMissing_Throws()
    {
        var partWithMissingReplacement = CreatePart("FL-1", supersededByPartId: Guid.NewGuid());
        var catalogue = new InMemoryPartCatalogue([partWithMissingReplacement]);

        Assert.Throws<InvalidOperationException>(() => catalogue.GetSupersessionChain("FL-1"));
    }

    private static Part CreatePart(string partNumber, Guid? id = null, Guid? supersededByPartId = null) =>
        new()
        {
            Id = id ?? Guid.NewGuid(),
            PartNumber = partNumber,
            Description = "Oil filter",
            Brand = "Corvane",
            Category = PartCategory.Filters,
            TradePriceGbp = 6.20m,
            RetailPriceGbp = 8.37m,
            StockQuantity = 1,
            IsDiscontinued = false,
            SupersededByPartId = supersededByPartId
        };
}