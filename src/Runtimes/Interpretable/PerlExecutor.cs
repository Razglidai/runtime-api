[ExecutorsAttribute("perl")]

public class PerlExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return InterpretRunner.Run((new RunnerData(LangRunnerConstants.PERL_INTERPRETER, "%f", ".pl")), request);
    }
}
