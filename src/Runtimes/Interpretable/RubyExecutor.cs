[ExecutorsAttribute("ruby")]

public class RubyExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return InterpretRunner.Run((new RunnerData(LangRunnerConstants.RUBY_INTERPRETER, "%f", ".rb")), request);
    }
}