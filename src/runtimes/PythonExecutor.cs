

[ExecutorsAttribute("python")]
public class PythonExecutor : IRuntimeExecutor
{
    public RuntimeResponse Execute(RuntimeRequest request)
    {
        
        return new RuntimeResponse("python", "noimpl", ["noimpl"]);
    }
}
