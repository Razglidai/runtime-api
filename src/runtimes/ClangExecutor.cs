using Microsoft.Scripting.Hosting;
public class ClangExecutor : IRuntimeExecutor
{
    private ScriptEngine _engine = Python.CreateEngine();
    public RuntimeResponse Execute(RuntimeRequest request)
    {
        
    }
}
