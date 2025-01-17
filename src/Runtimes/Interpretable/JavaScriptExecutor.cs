[ExecutorsAttribute("javascript")]

public class JavaScriptExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return InterpretRunner.Run((new RunnerData(LangRunnerConstants.NODE_JS_INTERPRETER, "%f", ".js")), request);
    }
}
