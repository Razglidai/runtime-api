using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

[ExecutersAttribute("python")]
public class PythonExecuter : IRuntimeExecuter
{
    private ScriptEngine _engine = Python.CreateEngine();
    public RuntimeResponse Execute(RuntimeRequest request)
    {
        var scope = _engine.CreateScope();
        _engine.Execute(request.code, scope);
        return new RuntimeResponse(scope.GetVariable("result").ToString(), "noimpl","noimpl");
    }
}
