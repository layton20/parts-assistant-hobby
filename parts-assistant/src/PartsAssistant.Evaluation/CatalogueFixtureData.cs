namespace PartsAssistant.Evaluation;

public sealed record CatalogueFixtureData(
    IReadOnlyList<VehicleFixture> Vehicles,
    IReadOnlyList<PartFixture> Parts);