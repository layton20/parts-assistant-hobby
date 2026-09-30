namespace PartsAssistant.Evaluation;

public interface IEvaluationRunObserver
{
    IEvaluationItemScope BeginItem(EvaluationRunInfo runInfo, EvaluationCase evaluationCase);
}
