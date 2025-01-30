using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

public static class InterpretRunner
{
    private static readonly object syncLock = new object();

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
        long runRAM = 0; 
        long runTime = 0;
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

                Task monitorTask = Task.Run(() =>
                {
                    try
                    {
                        while (!process.HasExited)
                        {
                            lock (syncLock)
                            {
                                runTime = (long)process.UserProcessorTime.TotalMilliseconds;
                                runRAM = process.WorkingSet64 / 1024;
                            }
                            Thread.Sleep(5);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка мониторинга: {ex.Message}");
                    }
                });

                using (StreamWriter writer = process.StandardInput)
                {
                    writer.WriteLine(stdin);
                    writer.Flush();
                    writer.Close();
                }

                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();

                process.WaitForExit(1000);
                monitorTask.Wait();

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
