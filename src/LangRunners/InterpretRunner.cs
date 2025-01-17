using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

public static class InterpretRunner
{
    private static readonly object randLock = new object();

    public static List<RuntimeDTO> Run(RunnerData interpreter, RuntimeRequest request)
    {
        List<RuntimeDTO> output = new List<RuntimeDTO>();
        string filenameBase = Guid.NewGuid().ToString("N");
        string sourceFilePath = Path.Combine(LangRunnerConstants.POOL_DIR, filenameBase + interpreter.sourceExtension);

        File.WriteAllText(sourceFilePath, request.code);

        foreach (string input in request.input)
        {
            RuntimeDTO result = Execute(interpreter, sourceFilePath, input);
            output.Add(result);
        }

        Cleanup(sourceFilePath);
        return output;
    }

    private static RuntimeDTO Execute(RunnerData interpreter, string sourceFilePath, string stdin)
    {
        string stdout = "", stderr = "";
        double runRAM = 0, runTime = 0;
        int exitCode = -1;

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = interpreter.execPath,
                Arguments = interpreter.buildPrompt.Replace("%f", sourceFilePath),
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
            stderr += "\nОшибка выполнения: " + e.StackTrace;
        }

        return new RuntimeDTO(stdout, stderr, exitCode, runTime, runRAM);
    }

    private static void Cleanup(string sourceFilePath)
    {
        try
        {
            if (File.Exists(sourceFilePath))
                File.Delete(sourceFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при удалении файлов: " + e.Message);
        }
    }
}
