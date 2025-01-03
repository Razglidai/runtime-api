using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

[ExecutorsAttribute("python")]
public class PythonExecutor : IRuntimeExecutor
{
    private ScriptEngine _engine = Python.CreateEngine();
    public RuntimeResponse Execute(RuntimeRequest request)
    {
        var scope = _engine.CreateScope();
        _engine.Execute(request.code, scope);
        return new RuntimeResponse(scope.GetVariable("result").ToString(), "noimpl","noimpl");
    }
}
