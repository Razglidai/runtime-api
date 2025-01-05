using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

[ExecutorsAttribute("clang")]
public class CExecutor : IRuntimeExecutor
{
    private readonly ILangService _service;

    public CExecutor()
    {
        _service = new CLangService();
    }

    public RuntimeResponse Execute(RuntimeRequest request)
    {
        Dictionary<string, string> BuildOutput = [];
        Dictionary<string, string> ExecOutput = [];

        string BuildArgs;
        if (!request.Input.TryGetValue("compile-args", out BuildArgs))
        {
            BuildArgs = "";
        }
        string ExecArgs;
        if (!request.Input.TryGetValue("exec-args", out ExecArgs))
        {
            ExecArgs = "";
        }

        int compilationExitCode = _service.Build(request.Code, BuildArgs, BuildOutput);

        if (compilationExitCode != 0)
        {
            _service.Cleanup();
            return new RuntimeResponse("clang", request.UUID, BuildOutput);
        }

        int executionExitCode = _service.Execute(ExecArgs, ExecOutput);
        _service.Cleanup();

        return new RuntimeResponse("clang", request.UUID, ExecOutput);
    }
}
