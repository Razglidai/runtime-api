[ExecutorsAttribute("clang")]
public class ClangExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return CompilerRunner.Run(new RunnerData(LangRunnerConstants.C_COMPILER, "-o %b %s", "c"), request);
    }
}
