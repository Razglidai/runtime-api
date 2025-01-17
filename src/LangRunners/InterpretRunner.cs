using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

 public static class InterpretRunner
{
    private static readonly object randLock = new object();

    private static string GenerateRandomString(uint length = 16)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] buffer = new char[length];

        Random localRandom;
        lock (randLock) localRandom = new Random();

        for (int i = 0; i < length; i++)
        {
            buffer[i] = chars[localRandom.Next(chars.Length)];
        }

        return new string(buffer);
    }

    public static List<RuntimeDTO> Run(RunnerData interpreter, RuntimeRequest request)
    {
        List<RuntimeDTO> output = new List<RuntimeDTO>();
        string filenameBase = GenerateRandomString();
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
            Process process = new Process();
            process.StartInfo.FileName = interpreter.execPath;
            process.StartInfo.Arguments = interpreter.buildPrompt.Replace("%f", sourceFilePath);
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
