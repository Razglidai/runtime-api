[ExecutorsAttribute("dlang")]
public class DlangExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return CompilerRunner.Run(new RunnerData(LangRunnerConstants.D_COMPILER, "-o %b %s", "d"), request);
    }
}
