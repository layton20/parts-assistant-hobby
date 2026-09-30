namespace PartsAssistant.Domain;

public sealed class InMemoryPartCatalogue : IPartCatalogue
{
    private readonly Dictionary<string, Part> partsByNumber;
    private readonly Dictionary<Guid, Part> partsById;

    public InMemoryPartCatalogue(IEnumerable<Part> parts)
    {
        var partList = parts.ToList();
        partsByNumber = partList.ToDictionary(p => p.PartNumber.Trim(), StringComparer.OrdinalIgnoreCase);
        partsById = partList.ToDictionary(p => p.Id);
    }

    public Part? FindByPartNumber(string partNumber)
    {
        return partsByNumber.TryGetValue(partNumber.Trim(), out var part) ? part : null;
    }

    public IReadOnlyList<Part> GetSupersessionChain(string partNumber)
    {
        var startingPart = FindByPartNumber(partNumber);

        if (startingPart == null)
        {
            return [];
        }

        var chain = new List<Part>();
        var visitedPartIds = new HashSet<Guid>();

        var currentPart = startingPart;
        while (currentPart is not null)
        {
            if (!visitedPartIds.Add(currentPart.Id))
            {
                throw new InvalidOperationException("A cycle was found in the part supersession chain.");
            }

            chain.Add(currentPart);
            currentPart = FindSupersedingPart(currentPart);
        }

        return chain;
    }

    private Part? FindSupersedingPart(Part part)
    {
        if (!part.SupersededByPartId.HasValue)
        {
            return null;
        }

        if (partsById.TryGetValue(part.SupersededByPartId.Value, out var supersedingPart))
        {
            return supersedingPart;
        }

        throw new InvalidOperationException($"Superseding part '{part.SupersededByPartId.Value}' was not found.");
    }
}