namespace PartsAssistant.Evaluation;

public static class EvaluationReportFormatter
{
    public static string Format(EvaluationReport report)
    {
        var categoryResults = report.Results
            .GroupBy(result => result.EvaluationCase.Category)
            .OrderBy(group => group.Key)
            .ToList();
        var categoryNameWidth = categoryResults
            .Select(group => group.Key.ToString().Length)
            .DefaultIfEmpty(0)
            .Max();
        var lines = new List<string>
        {
            "Evaluation Report",
            "=================",
            FormattableString.Invariant($"Overall: {report.PassedCount}/{report.Results.Count} passed ({report.PassRate * 100:0}%)"),
            string.Empty,
            "By category:"
        };

        foreach (var category in categoryResults)
        {
            var passedCount = category.Count(result => result.Score.Passed);
            var categoryName = category.Key.ToString().PadRight(categoryNameWidth);
            lines.Add(FormattableString.Invariant(
                $"  {categoryName}  {passedCount}/{category.Count()} passed ({(double)passedCount / category.Count() * 100:0}%)"));
        }

        var failedResults = report.FailedResults.ToList();
        if (failedResults.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("Failures:");

            foreach (var failedResult in failedResults)
            {
                lines.Add($"  {failedResult.EvaluationCase.Id}");
                lines.AddRange(failedResult.Score.FailureReasons.Select(reason => $"    - {reason}"));
            }
        }

        return string.Join(Environment.NewLine, lines);
    }
}