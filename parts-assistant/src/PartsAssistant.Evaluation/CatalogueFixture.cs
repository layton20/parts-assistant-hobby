using PartsAssistant.Domain;

namespace PartsAssistant.Evaluation;

public sealed record CatalogueFixture(
    IReadOnlyList<Vehicle> Vehicles,
    IReadOnlyList<Part> Parts,
    IReadOnlyList<PartFitment> Fitments);