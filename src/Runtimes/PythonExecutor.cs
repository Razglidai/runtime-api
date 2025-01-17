using Microsoft.Scripting.Hosting;

[ExecutorsAttribute("python")]
public class PythonExecutor : IRuntimeExecutor
{

    public PythonExecutor()
    {
    }

    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return null;
    }
}
