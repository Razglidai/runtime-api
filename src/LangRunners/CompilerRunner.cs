
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
    public static List<RuntimeDTO> Run(RunnerData compiler, RuntimeRequest request)
    {
        List<RuntimeDTO> output = new List<RuntimeDTO>();
        string filenameBase = Guid.NewGuid().ToString("N");
        string sourceFilePath = Path.Combine(LangRunnerConstants.POOL_DIR, filenameBase + "." + compiler.sourceExtension);
        string binaryFilePath = Path.Combine(LangRunnerConstants.POOL_DIR, filenameBase + ".exe");

        File.WriteAllText(sourceFilePath, request.code);

        foreach (string input in request.input)
        {
            RuntimeDTO result = Compile(compiler, sourceFilePath, binaryFilePath);
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
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = binaryFilePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                using (StreamWriter writer = process.StandardInput)
                {
                    writer.WriteLine(stdin);
                }

                TimeSpan processUserTime = TimeSpan.Zero;
                long processRAM = 0;

                Task monitorTask = Task.Run(() =>
                {
                    while (!process.HasExited)
                    {
                        processUserTime = process.TotalProcessorTime;
                        processRAM = process.WorkingSet64;
                        Thread.Sleep(0); // омега фикс 3000
                    }
                });

                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();


                process.WaitForExit();

                runTime = processUserTime.TotalMilliseconds;  // В миллисекундах
                runRAM = processRAM / 1024; // в КБ
                exitCode = process.ExitCode;
            }
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
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = compiler.execPath,
                Arguments = compiler.buildPrompt
                .Replace("%s", $"\"{sourceFilePath}\"")
                .Replace("%b", $"\"{binaryFilePath}\""),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                TimeSpan processUserTime = TimeSpan.Zero;
                long processRAM = 0;

                Task monitorTask = Task.Run(() =>
                {
                    while (!process.HasExited)
                    {
                        processUserTime = process.TotalProcessorTime;
                        processRAM = process.WorkingSet64;
                        Thread.Sleep(0); // омега фикс 3000
                    }
                });

                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();

                process.WaitForExit();

                runTime = processUserTime.TotalMilliseconds;  // В миллисекундах
                runRAM = processRAM / 1024; // в КБ
                exitCode = process.ExitCode;
            }
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
