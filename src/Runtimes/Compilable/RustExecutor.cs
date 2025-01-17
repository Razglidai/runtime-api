[ExecutorsAttribute("rust")]
public class RustExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return CompilerRunner.Run(new RunnerData(LangRunnerConstants.RUST_COMPILER, "-o %b %s", "rs"), request);
    }
}