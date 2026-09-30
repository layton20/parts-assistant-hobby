using System.Globalization;
using System.Text;
using PartsAssistant.Domain;

namespace PartsAssistant.OpenAi;

// Renders the part catalogue as a flat, line-per-part text block that gets
// embedded in the OpenAI prompt so the model has the data it needs to answer.
public static class CatalogueContextFormatter
{
    private const string NoReplacementOnRecord = "discontinued, no replacement on record";

    public static string Format(IReadOnlyList<Part> parts, IReadOnlyList<Vehicle> vehicles, IReadOnlyList<PartFitment> fitments)
    {
        // Fitments only carry Guid ids, so build lookups up front to avoid
        // re-scanning parts/vehicles for every fitment row below.

        // Part id -> part number, e.g. Guid("a1b2...") -> "BP-1042".
        // Used to resolve SupersededByPartId into a human-readable part number.
        var partNumbersById = parts.ToDictionary(part => part.Id, part => part.PartNumber);

        // Vehicle id -> rendered description, e.g. Guid("c3d4...") -> "Ford Focus 1.6 TDCi (2011-2018)".
        var vehicleDescriptionsById = vehicles.ToDictionary(vehicle => vehicle.Id, DescribeVehicle);

        // Part id -> sorted list of the vehicle descriptions it fits, e.g.
        // Guid("a1b2...") -> ["Ford Focus 1.6 TDCi (2011-2018)", "Ford Focus 2.0 TDCi (2011-2018)"].
        var fittingVehicleDescriptionsByPartId = fitments
            .GroupBy(fitment => fitment.PartId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(fitment => vehicleDescriptionsById[fitment.VehicleId])
                    .OrderBy(description => description, StringComparer.Ordinal)
                    .ToList());

        var builder = new StringBuilder();
        
        foreach (var part in parts.OrderBy(part => part.PartNumber, StringComparer.Ordinal))
        {
            var fittingVehicles = fittingVehicleDescriptionsByPartId.GetValueOrDefault(part.Id, []);
            builder.AppendLine(DescribePart(part, fittingVehicles, partNumbersById));
        }

        return builder.ToString();
    }

    // Renders one catalogue line, e.g.:
    // "BP-1042 | Front Brake Pads | Bosch | Brakes | trade £18.50 | retail £29.99 | stock 12 | current | fits: Ford Focus 1.6 TDCi (2011-2018)"
    private static string DescribePart(
        Part part,
        IReadOnlyList<string> fittingVehicles,
        IReadOnlyDictionary<Guid, string> partNumbersById)
    {
        var tradePrice = part.TradePriceGbp.ToString("F2", CultureInfo.InvariantCulture);
        var retailPrice = part.RetailPriceGbp.ToString("F2", CultureInfo.InvariantCulture);
        var status = DescribeStatus(part, partNumbersById);

        return $"{part.PartNumber} | {part.Description} | {part.Brand} | {part.Category} | " +
               $"trade £{tradePrice} | retail £{retailPrice} | stock {part.StockQuantity} | " +
               $"{status} | fits: {string.Join(", ", fittingVehicles)}";
    }

    // Renders a part's lifecycle status, e.g. "current",
    // "discontinued, superseded by BP-1043", or "discontinued, no replacement on record".
    private static string DescribeStatus(Part part, IReadOnlyDictionary<Guid, string> partNumbersById)
    {
        if (!part.IsDiscontinued)
        {
            return "current";
        }

        return part.SupersededByPartId is { } supersedingPartId
            ? $"discontinued, superseded by {partNumbersById[supersedingPartId]}"
            : NoReplacementOnRecord;
    }

    // Renders a vehicle, e.g. "Ford Focus 1.6 TDCi (2011-2018)".
    private static string DescribeVehicle(Vehicle vehicle) =>
        $"{vehicle.Make} {vehicle.Model} {vehicle.Engine} ({vehicle.YearFrom}-{vehicle.YearTo})";
}
