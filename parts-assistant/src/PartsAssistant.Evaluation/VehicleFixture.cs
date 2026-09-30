namespace PartsAssistant.Evaluation;

public sealed record VehicleFixture(
    string Key,
    string Make,
    string Model,
    string Engine,
    int YearFrom,
    int YearTo);