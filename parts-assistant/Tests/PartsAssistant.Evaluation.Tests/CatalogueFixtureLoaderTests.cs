namespace PartsAssistant.Evaluation.Tests;

public sealed class CatalogueFixtureLoaderTests
{
    [Fact]
    public void LoadFromJson_WithDuplicatePartNumber_Throws()
    {
        var json = CreateCatalogueJson(CreatePartJson("BP-1"), CreatePartJson("BP-1"));

        Assert.Throws<InvalidDataException>(() => CatalogueFixtureLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_WithUnknownSupersedingPart_Throws()
    {
        var json = CreateCatalogueJson(CreatePartJson("BP-1", supersededByPartNumberJson: "\"BP-MISSING\""));

        Assert.Throws<InvalidDataException>(() => CatalogueFixtureLoader.LoadFromJson(json));
    }

    private static string CreateCatalogueJson(params string[] partJsonItems) =>
        $$"""{ "vehicles": [], "parts": [{{string.Join(",", partJsonItems)}}] }""";

    private static string CreatePartJson(string partNumber, string supersededByPartNumberJson = "null") =>
        $$"""
        {
          "partNumber": "{{partNumber}}",
          "description": "Test part",
          "brand": "Vantex",
          "category": "brakes",
          "tradePriceGbp": 10.00,
          "retailPriceGbp": 13.50,
          "stockQuantity": 1,
          "isDiscontinued": false,
          "supersededByPartNumber": {{supersededByPartNumberJson}},
          "fitsVehicleKeys": []
        }
        """;
}