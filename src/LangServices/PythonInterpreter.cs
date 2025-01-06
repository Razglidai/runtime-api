using System.Diagnostics;
using System.Text;
using IronPython.Runtime.Operations;

public class PythonInterpreter : GenericInterpreter
{

    public override int Execute(string Code, Dictionary<string, string> Output)
    {
        int exit;
        sourceFileName = generateFilename() + ".py";

        // Запись кода в файл
        File.WriteAllText(sourceFileName, Code);


        var executionProcess = new ProcessStartInfo
        {
            FileName = RuntimeConstants.PythonInterpreterPath,
            Arguments = sourceFileName,
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
