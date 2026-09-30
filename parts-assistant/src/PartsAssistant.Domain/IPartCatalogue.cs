namespace PartsAssistant.Domain;

public interface IPartCatalogue
{
    Part? FindByPartNumber(string partNumber);
    IReadOnlyList<Part> GetSupersessionChain(string partNumber);
}