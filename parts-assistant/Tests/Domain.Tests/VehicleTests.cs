using PartsAssistant.Domain;

namespace Domain.Tests;

public sealed class VehicleTests
{
    private const int FirstProductionYear = 2015;
    private const int LastProductionYear = 2019;

    [Theory]
    [InlineData(FirstProductionYear - 1, false)]
    [InlineData(FirstProductionYear, true)]
    [InlineData(LastProductionYear, true)]
    [InlineData(LastProductionYear + 1, false)]
    public void IsProducedIn_ChecksInclusiveYearRange(int year, bool expectedIsProduced)
    {
        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Make = "Halden",
            Model = "Corvid",
            Engine = "1.6 Petrol",
            YearFrom = FirstProductionYear,
            YearTo = LastProductionYear
        };

        Assert.Equal(expectedIsProduced, vehicle.IsProducedIn(year));
    }
}