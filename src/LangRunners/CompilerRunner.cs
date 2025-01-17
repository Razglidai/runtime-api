
using System.CodeDom.Compiler;
using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

public static class CompilerRunner
{
    private static readonly Random random = new Random();
    private static readonly object randLock = new object();

    public static List<RuntimeDTO> Run(RunnerData compiler, RuntimeRequest request)
    {
        List<RuntimeDTO> output = new List<RuntimeDTO>();
        string filenameBase = Guid.NewGuid().ToString();
        string sourceFilePath = Path.Combine(LangRunnerConstants.POOL_DIR, filenameBase + "." + compiler.sourceExtension);
        string binaryFilePath = Path.Combine(LangRunnerConstants.POOL_DIR, filenameBase + ".exe");

        File.WriteAllText(sourceFilePath, request.code);

        foreach (string input in request.input)
        {
            RuntimeDTO result = Compile(compiler,sourceFilePath, binaryFilePath);
            if (result.exitCode != 0)
            {
                output.Add(result);
                continue;
            }
            result = Execute(binaryFilePath, input);
            output.Add(result);
        }

        Cleanup(sourceFilePath, binaryFilePath);
        return output;
    }

    private static RuntimeDTO Execute(string binaryFilePath, string stdin)
    {
        string stdout = "", stderr = "";
        double runRAM = 0, runTime = 0;
        int exitCode = -1;

        try
        {
            Process process = new Process();
            process.StartInfo.FileName = binaryFilePath;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            Stopwatch stopwatch = Stopwatch.StartNew();
            process.Start();

            using (StreamWriter writer = process.StandardInput)
            {
                writer.WriteLine(stdin);
                writer.Close();
            }

            stdout = process.StandardOutput.ReadToEnd();
            stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            stopwatch.Stop();
            exitCode = process.ExitCode;

            runTime = stopwatch.Elapsed.TotalMilliseconds;
        }
        catch (Exception e)
        {
            stderr += "\nОшибка выполнения: " + e.Message;
        }

        return new RuntimeDTO(stdout, stderr, exitCode, runTime, runRAM);
    }

    private static RuntimeDTO Compile(RunnerData compiler, string sourceFilePath, string binaryFilePath)
    {
        string stdout = "", stderr = "";
        double runRAM = 0, runTime = 0;
        int exitCode = -1;

        try
        {
            Process process = new Process();
            process.StartInfo.FileName = compiler.execPath;
            process.StartInfo.Arguments = compiler.buildPrompt
                .Replace("%s", $"\"{sourceFilePath}\"")
                .Replace("%b", $"\"{binaryFilePath}\"");
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            Stopwatch stopwatch = Stopwatch.StartNew();
            process.Start();

            stdout = process.StandardOutput.ReadToEnd();
            stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            stopwatch.Stop();
            exitCode = process.ExitCode;

            runTime = stopwatch.Elapsed.TotalMilliseconds;
        }
        catch (Exception e)
        {
            stderr += "\nОшибка компиляции: " + e.Message;
        }

        return new RuntimeDTO(stdout, stderr, exitCode, runTime, runRAM);
    }

    private static void Cleanup(string sourceFilePath, string binaryFilePath)
    {
        try
        {
            if (File.Exists(sourceFilePath))
                File.Delete(sourceFilePath);
            if (File.Exists(binaryFilePath))
                File.Delete(binaryFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при удалении файлов: " + e.Message);
        }
    }
}
