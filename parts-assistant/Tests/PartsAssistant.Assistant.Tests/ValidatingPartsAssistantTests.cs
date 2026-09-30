using PartsAssistant.Domain;
using PartsAssistant.Domain.Enums;

namespace PartsAssistant.Assistant.Tests;

public sealed class ValidatingPartsAssistantTests
{
    [Fact]
    public async Task AskAsync_WhenInnerAssistantAbstains_PassesTheResponseThroughUnchanged()
    {
        var innerResponse = new AssistantResponse(AssistantOutcome.Abstain, [], "I can't help with that.");
        var validator = CreateValidator(innerResponse, catalogue: []);

        var response = await validator.AskAsync("anything", CancellationToken.None);

        Assert.Equal(innerResponse, response);
    }

    [Fact]
    public async Task AskAsync_WhenRecommendedPartIsCurrent_PassesTheResponseThroughUnchanged()
    {
        var currentPart = CreatePart("FL-3002");
        var innerResponse = new AssistantResponse(AssistantOutcome.Answer, [currentPart.PartNumber], "Here you go.");
        var validator = CreateValidator(innerResponse, catalogue: [currentPart]);

        var response = await validator.AskAsync("anything", CancellationToken.None);

        Assert.Equal(innerResponse, response);
    }

    [Fact]
    public async Task AskAsync_WhenRecommendedPartDoesNotExist_AbstainsInstead()
    {
        var innerResponse = new AssistantResponse(AssistantOutcome.Answer, ["BP-0000-X"], "Here you go.");
        var validator = CreateValidator(innerResponse, catalogue: []);

        var response = await validator.AskAsync("anything", CancellationToken.None);

        Assert.Equal(AssistantOutcome.Abstain, response.Outcome);
        Assert.Empty(response.PartNumbers);
    }

    [Fact]
    public async Task AskAsync_WhenRecommendedPartIsDiscontinued_AbstainsEvenThoughAReplacementIsOnRecord()
    {
        var currentPart = CreatePart("FL-3002");
        var discontinuedPart = CreatePart("FL-3000", isDiscontinued: true, supersededByPartId: currentPart.Id);
        var innerResponse = new AssistantResponse(AssistantOutcome.Answer, [discontinuedPart.PartNumber], "Here you go.");
        var validator = CreateValidator(innerResponse, catalogue: [discontinuedPart, currentPart]);

        var response = await validator.AskAsync("anything", CancellationToken.None);

        Assert.Equal(AssistantOutcome.Abstain, response.Outcome);
    }

    [Fact]
    public async Task AskAsync_WhenOutcomeIsAnswerWithNoPartNumbers_AbstainsInstead()
    {
        var innerResponse = new AssistantResponse(AssistantOutcome.Answer, [], "Here you go.");
        var validator = CreateValidator(innerResponse, catalogue: []);

        var response = await validator.AskAsync("anything", CancellationToken.None);

        Assert.Equal(AssistantOutcome.Abstain, response.Outcome);
    }

    private static ValidatingPartsAssistant CreateValidator(AssistantResponse innerResponse, IReadOnlyList<Part> catalogue) =>
        new(new FixedResponseAssistant(innerResponse), new InMemoryPartCatalogue(catalogue));

    private static Part CreatePart(string partNumber, bool isDiscontinued = false, Guid? supersededByPartId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            PartNumber = partNumber,
            Description = "Test part",
            Brand = "Vantex",
            Category = PartCategory.Filters,
            TradePriceGbp = 6.20m,
            RetailPriceGbp = 8.37m,
            StockQuantity = 1,
            IsDiscontinued = isDiscontinued,
            SupersededByPartId = supersededByPartId
        };

    private sealed class FixedResponseAssistant(AssistantResponse response) : IPartsAssistant
    {
        public Task<AssistantResponse> AskAsync(string question, CancellationToken cancellationToken) =>
            Task.FromResult(response);
    }
}
