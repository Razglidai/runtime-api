using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

[ExecutorsAttribute("clang")]
public class ClangExecutor : IRuntimeExecutor
{

    private string compilationSTDOUT = "";
    private string compilationSTDERR = "";
    private int compilationExitCode = -1;
    private string executionSTDOUT = "";
    private string executionSTDERR = "";
    private int executionExitCode = -1;
    string sourceFileName = "";
    private string binaryFileName = "";

    private int CompileSourceCode(string code)
    {
        Random rand = new();
        string filename = rand.Next(0, 1000000000).ToString();
        sourceFileName = filename + ".c";
        binaryFileName = filename + ".elf";


        File.WriteAllText(sourceFileName, code);

        var compilerProcess = new ProcessStartInfo
        {
            FileName = RuntimeConstants.ClangCompilerPath,
            Arguments = $"{sourceFileName} -o {binaryFileName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = compilerProcess })
        {
            process.Start();
            compilationSTDOUT = process.StandardOutput.ReadToEnd();
            compilationSTDERR = process.StandardError.ReadToEnd();
            process.WaitForExit();

            compilationExitCode = process.ExitCode;
        }
        return compilationExitCode;
    }

    private int ExecuteBinaryFile()
    {
        var executionProcess = new ProcessStartInfo
        {
            FileName = RuntimeConstants.ShellPath, // Using /bin/sh to execute the binary
            Arguments = $"-c ./{binaryFileName}", // Properly call the binary
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = executionProcess })
        {
            process.Start();
            executionSTDOUT = process.StandardOutput.ReadToEnd();
            executionSTDERR = process.StandardError.ReadToEnd();
            process.WaitForExit();
            executionExitCode = process.ExitCode;
        }
        return executionExitCode;
    }

    void Cleanup() {
        if(File.Exists(sourceFileName)) {
            File.Delete(sourceFileName);
        }
        if(File.Exists(binaryFileName)) {
            File.Delete(binaryFileName);
        }
    }

    public RuntimeResponse Execute(RuntimeRequest request)
    {
        int compilationExitCode = CompileSourceCode(request.Code);
        ICollection<string> output = [];
        Guid guid = Guid.NewGuid();

        output.Add(compilationSTDOUT);
        output.Add(compilationSTDERR);

        if (compilationExitCode != 0)
        {
            Cleanup();
            return new RuntimeResponse("clang", guid.ToString(), output);
        }
        
        int executionExitCode = ExecuteBinaryFile();
        output.Add(executionSTDOUT);
        output.Add(executionSTDERR);
        Cleanup();

        return new RuntimeResponse("clang", guid.ToString(), output);
    }

    ~ClangExecutor() {
    }
}
