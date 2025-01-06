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
        File.WriteAllText(sourceFileName, Encoding.UTF8.GetString(Convert.FromBase64String(Code)));


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
            Output["stdout"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(process.StandardOutput.ReadToEnd()));
            Output["stderr"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(process.StandardError.ReadToEnd()));
            process.WaitForExit();
            exit = process.ExitCode;
        }
        Output["exit"] = exit.ToString();
        return exit;
    }
}
