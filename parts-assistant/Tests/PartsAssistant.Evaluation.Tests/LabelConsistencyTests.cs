using PartsAssistant.Domain;

namespace PartsAssistant.Evaluation.Tests;

public sealed class LabelConsistencyTests
{
    private const int RequestedYearOutsideFitmentRange = 2019;

    private static readonly CatalogueFixture Catalogue =
        CatalogueFixtureLoader.LoadFromFile(DatasetPath("catalogue-fixture.json"));

    private static readonly IReadOnlyList<EvaluationCase> Cases =
        EvaluationDatasetLoader.LoadFromFile(DatasetPath("evaluation-cases.json"));

    [Fact]
    public void EveryLabelledPartNumberExistsInTheCatalogue()
    {
        var knownPartNumbers = Catalogue.Parts.Select(part => part.PartNumber).ToHashSet();
        var labelledPartNumbers = Cases.SelectMany(
            evaluationCase => evaluationCase.RequiredPartNumbers.Concat(evaluationCase.ForbiddenPartNumbers));

        Assert.Empty(labelledPartNumbers.Except(knownPartNumbers));
    }

    [Fact]
    public void NearIdenticalCase_BothPadsFitTheRequestedVehicle()
    {
        var evaluationCase = FindCase("front-pads-not-rear-pads");
        var requestedVehicle = FindVehicle("Halden", "Corvid", "1.6 Petrol");

        Assert.True(requestedVehicle.IsProducedIn(2017));
        Assert.All(
            evaluationCase.RequiredPartNumbers.Concat(evaluationCase.ForbiddenPartNumbers),
            partNumber => Assert.True(Fits(partNumber, requestedVehicle)));
    }

    [Fact]
    public void SupersessionCase_RequiredPartIsTheCurrentEndOfTheChain()
    {
        var evaluationCase = FindCase("multi-hop-supersession");
        var chain = new InMemoryPartCatalogue(Catalogue.Parts).GetSupersessionChain("FL-3000");

        Assert.Equal(evaluationCase.RequiredPartNumbers.Single(), chain[^1].PartNumber);
        Assert.False(chain[^1].IsDiscontinued);
    }

    [Fact]
    public void OutOfStockCase_OriginalHasNoStockAndAlternativeFitsTheSameVehicle()
    {
        var evaluationCase = FindCase("out-of-stock-with-alternative");
        var originalPart = FindPart("FL-5100");
        var alternativePart = FindPart(evaluationCase.RequiredPartNumbers.Single());
        var requestedVehicle = FindVehicle("Ridgemont", "Tundra", "3.0 Diesel");

        Assert.Equal(0, originalPart.StockQuantity);
        Assert.True(alternativePart.StockQuantity > 0);
        Assert.True(Fits(originalPart.PartNumber, requestedVehicle));
        Assert.True(Fits(alternativePart.PartNumber, requestedVehicle));
    }

    [Fact]
    public void FuelTypeCase_SparkPlugsFitNoDieselAndGlowPlugDoes()
    {
        var evaluationCase = FindCase("spark-plug-for-diesel");
        var dieselVehicle = FindVehicle("Halden", "Corvid", "1.6 Diesel");

        Assert.All(evaluationCase.ForbiddenPartNumbers, partNumber => Assert.False(Fits(partNumber, dieselVehicle)));
        Assert.All(evaluationCase.RequiredPartNumbers, partNumber => Assert.True(Fits(partNumber, dieselVehicle)));
    }

    [Fact]
    public void YearRangeCase_ForbiddenPartFitsTheEngineButNotInTheRequestedYear()
    {
        var evaluationCase = FindCase("year-outside-fitment-range");
        var matchingVehicles = Catalogue.Vehicles
            .Where(vehicle => vehicle.Make == "Halden" && vehicle.Model == "Corvid" && vehicle.Engine == "2.0 Petrol")
            .ToList();

        Assert.NotEmpty(matchingVehicles);
        Assert.DoesNotContain(matchingVehicles, vehicle => vehicle.IsProducedIn(RequestedYearOutsideFitmentRange));
        Assert.All(
            evaluationCase.ForbiddenPartNumbers,
            partNumber => Assert.Contains(matchingVehicles, vehicle => Fits(partNumber, vehicle)));
    }

    private static string DatasetPath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "Dataset", fileName);

    private static EvaluationCase FindCase(string id) => Cases.Single(evaluationCase => evaluationCase.Id == id);

    private static Part FindPart(string partNumber) => Catalogue.Parts.Single(part => part.PartNumber == partNumber);

    private static Vehicle FindVehicle(string make, string model, string engine) =>
        Catalogue.Vehicles.Single(vehicle => vehicle.Make == make && vehicle.Model == model && vehicle.Engine == engine);

    private static bool Fits(string partNumber, Vehicle vehicle)
    {
        var partId = FindPart(partNumber).Id;
        return Catalogue.Fitments.Any(fitment => fitment.PartId == partId && fitment.VehicleId == vehicle.Id);
    }
}