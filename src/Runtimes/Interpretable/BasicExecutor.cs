[ExecutorsAttribute("basic")]

public class BasicExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return InterpretRunner.Run((new RunnerData(LangRunnerConstants.BASIC_INTERPRETER, "%f", ".bas")), request);
    }
}
