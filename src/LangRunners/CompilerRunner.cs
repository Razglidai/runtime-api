
using System.CodeDom.Compiler;
using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

// Код раннера компилируемых языков
// Где бог, когда он так нужен?
public static class CompilerRunner
{
    // Нужная часть с таском мониторинга памяти и ресурсов
    private static object syncLock = new object();

    // А чё она статическая?
    public static List<RuntimeDTO> Run(RunnerData compiler, RuntimeRequest request)
    {
        // Выводные данные
        List<RuntimeDTO> output = new List<RuntimeDTO>();

        // Собираем новый файл в который будет навалено кодом
        string filenameBase = Guid.NewGuid().ToString("N");
        string sourceFilePath = Path.Combine(LangRunnerConstants.POOL_DIR, filenameBase + "." + compiler.sourceExtension);
        string binaryFilePath = Path.Combine(LangRunnerConstants.POOL_DIR, filenameBase + ".exe");

        File.WriteAllText(sourceFilePath, request.code);

        // Так как у нас в реквесте массив вводов,
        // делаем цикл с ними и тестим код
        foreach (string input in request.input)
        {
            // компиляция
            RuntimeDTO result = Compile(compiler, sourceFilePath, binaryFilePath);
            // если не скомпилилось
            if (result.exitCode != 0)
            {
                output.Add(result);
                continue;
            }
            // прогон
            result = Execute(binaryFilePath, input);
            // финал
            output.Add(result);
        }

        Cleanup(sourceFilePath, binaryFilePath);
        return output;
    }

    private static RuntimeDTO Execute(string binaryFilePath, string stdin)
    {
        // Получаемые значения с прогона бинарника
        string stdout = "", stderr = "";
        long runRAM = 0;
        double runTime = 0;
        int exitCode = -1;

        // Богу стыдно за это
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

                // таск монитора, цепляется ВО ВРЕМЯ РАБОТЫ 
                // К ЗАПУЩЕННОМУ ПРОЦЕССУ
                // И РАБОТАЕТ ПОКА ТОТ ЗАПУЩЕН
                // ИНАЧЕ EXCEPTION

                Task monitorTask = Task.Run(() =>
                {
                    try
                    {
                        while (!process.HasExited)
                        {
                            lock (syncLock)
                            {
                                runTime = process.TotalProcessorTime.TotalMilliseconds;
                                runRAM = process.WorkingSet64 / 1024; // ловкий перевод в КБ (красное и белое)
                            }
                            /*
                            Одному богу на этой грешной земле известны
                            замыслы разработчиков того проклятого
                            Process, считающего время работы процесса
                            только во время выполнения и только
                            каждые 10 миллисекунд
                            Это также распространяется на stdout и stderr

                            короче код внизу ничего полезного не делает, но
                            предотвращает затирание данных при закрывшемся процессе
                            */
                            Thread.Sleep(5);
                        }
                    }
                    catch (Exception ex)
                    {
                        // просто отладка
                        Console.WriteLine($"Ошибка мониторинга: {ex.Message}");
                    }
                });

                // граббим выводы

                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();

                // ждём конца процесса
                process.WaitForExit();

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
        long runRAM = 0;
        double runTime = 0;
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

                Task monitorTask = Task.Run(() =>
                {
                    try
                    {
                        while (!process.HasExited)
                        {
                            lock (syncLock)
                            {
                                runTime = process.TotalProcessorTime.TotalMilliseconds;
                                runRAM = process.WorkingSet64 / 1024;
                            }
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка мониторинга: {ex.Message}");
                    }
                });

                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();

                process.WaitForExit(1000);

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
