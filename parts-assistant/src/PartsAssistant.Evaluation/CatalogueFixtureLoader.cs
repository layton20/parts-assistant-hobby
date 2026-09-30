using System.Text.Json;
using PartsAssistant.Domain;

namespace PartsAssistant.Evaluation;

public static class CatalogueFixtureLoader
{
    public static CatalogueFixture LoadFromFile(string path) =>
        LoadFromJson(File.ReadAllText(path));

    public static CatalogueFixture LoadFromJson(string json)
    {
        var data = JsonSerializer.Deserialize<CatalogueFixtureData>(json, EvaluationJsonOptions.Default)
            ?? throw new InvalidDataException("The catalogue fixture is empty.");

        EnsureNoDuplicates(data.Vehicles.Select(vehicle => vehicle.Key), "vehicle key");
        EnsureNoDuplicates(data.Parts.Select(part => part.PartNumber), "part number");

        var vehicleIdsByKey = data.Vehicles.ToDictionary(vehicle => vehicle.Key, _ => Guid.NewGuid());
        var partIdsByNumber = data.Parts.ToDictionary(part => part.PartNumber, _ => Guid.NewGuid());

        var vehicles = data.Vehicles
            .Select(vehicle => new Vehicle
            {
                Id = vehicleIdsByKey[vehicle.Key],
                Make = vehicle.Make,
                Model = vehicle.Model,
                Engine = vehicle.Engine,
                YearFrom = vehicle.YearFrom,
                YearTo = vehicle.YearTo
            })
            .ToList();

        var parts = data.Parts
            .Select(part => new Part
            {
                Id = partIdsByNumber[part.PartNumber],
                PartNumber = part.PartNumber,
                Description = part.Description,
                Brand = part.Brand,
                Category = part.Category,
                TradePriceGbp = part.TradePriceGbp,
                RetailPriceGbp = part.RetailPriceGbp,
                StockQuantity = part.StockQuantity,
                IsDiscontinued = part.IsDiscontinued,
                SupersededByPartId = ResolveSupersedingPartId(part, partIdsByNumber)
            })
            .ToList();

        var fitments = data.Parts
            .SelectMany(part => part.FitsVehicleKeys
                .Distinct()
                .Select(vehicleKey => new PartFitment
                {
                    PartId = partIdsByNumber[part.PartNumber],
                    VehicleId = ResolveVehicleId(vehicleKey, part.PartNumber, vehicleIdsByKey)
                }))
            .ToList();

        return new CatalogueFixture(vehicles, parts, fitments);
    }

    private static Guid? ResolveSupersedingPartId(PartFixture part, IReadOnlyDictionary<string, Guid> partIdsByNumber)
    {
        if (part.SupersededByPartNumber is null)
        {
            return null;
        }

        return partIdsByNumber.TryGetValue(part.SupersededByPartNumber, out var supersedingPartId)
            ? supersedingPartId
            : throw new InvalidDataException(
                $"Part {part.PartNumber} is superseded by unknown part {part.SupersededByPartNumber}.");
    }

    private static Guid ResolveVehicleId(string vehicleKey, string partNumber, IReadOnlyDictionary<string, Guid> vehicleIdsByKey) =>
        vehicleIdsByKey.TryGetValue(vehicleKey, out var vehicleId)
            ? vehicleId
            : throw new InvalidDataException($"Part {partNumber} fits unknown vehicle {vehicleKey}.");

    private static void EnsureNoDuplicates(IEnumerable<string> values, string description)
    {
        var duplicateGroup = values.GroupBy(value => value).FirstOrDefault(group => group.Count() > 1);
        if (duplicateGroup is not null)
        {
            throw new InvalidDataException($"Duplicate {description}: {duplicateGroup.Key}.");
        }
    }
}