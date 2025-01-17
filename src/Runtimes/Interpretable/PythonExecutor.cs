[ExecutorsAttribute("python")]
public class PythonExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return InterpretRunner.Run((new RunnerData(LangRunnerConstants.PYTHON_INTERPRETER, "%f", ".py")), request);
    }
}
