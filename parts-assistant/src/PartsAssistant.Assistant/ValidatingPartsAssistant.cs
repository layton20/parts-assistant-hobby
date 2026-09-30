using PartsAssistant.Domain;

namespace PartsAssistant.Assistant;

public sealed class ValidatingPartsAssistant(IPartsAssistant innerAssistant, IPartCatalogue catalogue) : IPartsAssistant
{
	private const string UnknownPartMessage =
	"I found a match, but I can't confirm it against the catalogue, so I don't want to guess. Please ask a colleague to check part {0}.";

	private const string DiscontinuedPartMessage =
		"The part I was about to recommend, {0}, is discontinued. Please ask a colleague to confirm the current replacement.";

	private const string EmptyAnswerMessage =
		"Something went wrong producing an answer. Please try asking again.";

	public async Task<AssistantResponse> AskAsync(string question, CancellationToken cancellationToken)
	{
		var response = await innerAssistant.AskAsync(question, cancellationToken);

		return response.Outcome == AssistantOutcome.Answer ? Validate(response) : response;
	}

	private AssistantResponse Validate(AssistantResponse response)
	{
		if (response.PartNumbers.Count == 0)
		{
			return Abstain(EmptyAnswerMessage);
		}

		foreach (var partNumber in response.PartNumbers)
		{
			var part = catalogue.FindByPartNumber(partNumber);

			if (part is null)
			{
				return Abstain(string.Format(UnknownPartMessage, partNumber));
			}

			if (part.IsDiscontinued)
			{
				return Abstain(string.Format(DiscontinuedPartMessage, partNumber));
			}
		}

		return response;
	}

	private static AssistantResponse Abstain(string message) => new(AssistantOutcome.Abstain, [], message);
}