using System.Diagnostics;
using System.Text;
using IronPython.Runtime.Operations;

public class CBuilder : GenericBuilder
{
    public override int Build(string Code, string CompileArgs, Dictionary<string, string> Output)
    {
        int exit;
        sourceFileName = generateFilename() + ".c";
        binaryFileName = generateFilename() + ".elf";

        if (!Directory.Exists(RuntimeConstants.ClangDirectory))
        {
            Directory.CreateDirectory(RuntimeConstants.ClangDirectory);
        }

        File.WriteAllText(sourceFileName, Code);

        var compilerProcess = new ProcessStartInfo
        {
            FileName = RuntimeConstants.ClangCompilerPath,
            Arguments = CompileArgs + " " + $"{sourceFileName} -o {binaryFileName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = compilerProcess })
        {
            process.Start();
            Output["stdout"] = process.StandardOutput.ReadToEnd();
            Output["stderr"] = process.StandardError.ReadToEnd();
            process.WaitForExit();

            exit = process.ExitCode;
        }
        Output["exit"] = exit.ToString();
        return exit;
    }

    public override int Execute(string ExecArgs, Dictionary<string, string> Output)
    {
        int exit;
        var executionProcess = new ProcessStartInfo
        {
            FileName = RuntimeConstants.ShellPath,
            Arguments = $"-c \"./{binaryFileName} {ExecArgs}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = executionProcess })
        {
            process.Start();
            Output["stdout"] = process.StandardOutput.ReadToEnd();
            Output["stderr"] = process.StandardError.ReadToEnd();
            process.WaitForExit();
            exit = process.ExitCode;
        }
        Output["exit"] = exit.ToString();
        return exit;
    }
}
