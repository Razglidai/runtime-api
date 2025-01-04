using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

[ExecutorsAttribute("clang")]
public class ClangExecutor : IRuntimeExecutor
{
    private readonly ILangService _service;

    public ClangExecutor()
    {
        _service = new CLangService();
    }

    public RuntimeResponse Execute(RuntimeRequest request)
    {
        Dictionary<string, string> BuildOutput = [];
        Dictionary<string, string> ExecOutput = [];

        int compilationExitCode = _service.Build(request.Code, new List<string>(), BuildOutput);

        if (compilationExitCode != 0)
        {
            _service.Cleanup();
            return new RuntimeResponse("clang", request.UUID, BuildOutput.Values);
        }

        int executionExitCode = _service.Execute(ExecOutput);
        _service.Cleanup();

        return new RuntimeResponse("clang", request.UUID, ExecOutput.Values);
    }
}
