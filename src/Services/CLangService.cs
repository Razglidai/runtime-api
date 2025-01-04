using System.Diagnostics;
using System.Numerics;
using Microsoft.AspNetCore.Authentication;

public class CLangService : ILangService
{    
    private string sourceFileName = "";
    private string binaryFileName = "";

    public int Build(string code, List<string> args, Dictionary<string, string> output)
    {
        int exit;
        Random rand = new();
        string filename = Path.Combine(RuntimeConstants.ClangDirectory, rand.Next(0, 1000000000).ToString());
        sourceFileName = filename + ".c";
        binaryFileName = filename + ".elf";

        File.WriteAllText(sourceFileName, code);

        var compilerProcess = new ProcessStartInfo
        {
            FileName = RuntimeConstants.ClangCompilerPath,
            Arguments = string.Join(" ", args) + " " + $"{sourceFileName} -o {binaryFileName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = compilerProcess })
        {
            process.Start();
            output["stdout"] = process.StandardOutput.ReadToEnd();
            output["stderr"] = process.StandardError.ReadToEnd();
            process.WaitForExit();

            exit = process.ExitCode;
        }
        output["exit"] = exit.ToString();
        return exit;
    }

    public int Execute(Dictionary<string, string> output)
    {
        int exit;

        var executionProcess = new ProcessStartInfo
        {
            FileName = RuntimeConstants.ShellPath,
            Arguments = $"-c ./{binaryFileName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = executionProcess })
        {
            process.Start();
            output["stdout"] = process.StandardOutput.ReadToEnd();
            output["stderr"] = process.StandardError.ReadToEnd();
            process.WaitForExit();
            exit = process.ExitCode;
        }
        output["exit"] = exit.ToString();
        return exit;
    }

    public void Cleanup()
    {
        if (File.Exists(sourceFileName))
        {
            File.Delete(sourceFileName);
        }
        if (File.Exists(binaryFileName))
        {
            File.Delete(binaryFileName);
        }
    }
}
