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
    private readonly GenericBuilder _service;

    public CExecutor()
    {
        _service = new CBuilder();
    }

    public RuntimeResponse Execute(RuntimeRequest request)
    {
        Dictionary<string, string> BuildOutput = [];
        Dictionary<string, string> ExecOutput = [];

        string BuildArgs=request.Input["build-args"] ?? "";

        string ExecArgs=request.Input["exec-args"] ?? "";

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
