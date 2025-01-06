using Microsoft.Scripting.Hosting;

[ExecutorsAttribute("python")]
public class PythonExecutor : IRuntimeExecutor
{
    private readonly GenericInterpreter _service;

    public PythonExecutor()
    {
        _service = new PythonInterpreter();
    }

    public RuntimeResponse Execute(RuntimeRequest request)
    {
        Dictionary<string, string> ExecOutput = new Dictionary<string, string>();

        int executionExitCode = _service.Execute(request.Code, ExecOutput);
        _service.Cleanup();

        return new RuntimeResponse("python", request.UUID, ExecOutput);
    }
}
