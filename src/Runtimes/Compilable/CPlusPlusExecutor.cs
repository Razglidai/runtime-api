[ExecutorsAttribute("cplusplus")]
public class CPlusPlusExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return CompilerRunner.Run(new RunnerData(LangRunnerConstants.CPP_COMPILER, "-o %b %s", "cpp"), request);
    }
}
