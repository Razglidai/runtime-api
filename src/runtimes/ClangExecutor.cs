using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using System.IO;

[ExecutorsAttribute("clang")]
public class ClangExecutor : IRuntimeExecutor
{

    private string compilationSTDOUT = "";
    private string compilationSTDERR = "";
    private int compilationExitCode = -1;
    private string executionSTDOUT = "";
    private string executionSTDERR = "";
    private int executionExitCode = -1;
    string sourceFileName = "";
    private string binaryFileName = "";

    private int CompileSourceCode(string code)
    {
        Random rand = new Random();
        string filename = rand.Next(0, 128).ToString();
        sourceFileName = filename + ".c";
        binaryFileName = filename + ".elf";


        File.WriteAllText(sourceFileName, code);

        var compilerProcess = new ProcessStartInfo
        {
            FileName = "/usr/bin/gcc",
            Arguments = $"{sourceFileName} -o {binaryFileName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = compilerProcess })
        {
            process.Start();
            compilationSTDOUT = process.StandardOutput.ReadToEnd();
            compilationSTDERR = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine("Compile Output: " + compilationSTDOUT); 
            Console.WriteLine("Compile Error: " + compilationSTDERR);

            compilationExitCode = process.ExitCode;
        }
        return compilationExitCode;
    }

    private int ExecuteBinaryFile()
    {
        var executionProcess = new ProcessStartInfo
        {
            FileName = $"/bin/sh", // Using /bin/sh to execute the binary
            Arguments = $"-c ./{binaryFileName}", // Properly call the binary
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = executionProcess })
        {
            process.Start();
            executionSTDOUT = process.StandardOutput.ReadToEnd();
            executionSTDERR = process.StandardError.ReadToEnd();
            process.WaitForExit();
            executionExitCode = process.ExitCode;
        }
        return executionExitCode;
    }

    void Cleanup() {
        if(File.Exists(sourceFileName)) {
            File.Delete(sourceFileName);
        }
        if(File.Exists(binaryFileName)) {
            File.Delete(binaryFileName);
        }
    }

    public RuntimeResponse Execute(RuntimeRequest request)
    {
        int compilationExitCode = CompileSourceCode(request.code);
        ICollection<string> output = new List<string>();
        Guid guid = Guid.NewGuid();

        output.Add(compilationSTDOUT);
        output.Add(compilationSTDERR);

        if (compilationExitCode != 0)
        {
            return new RuntimeResponse("clang", guid.ToString(), output);
        }
        
        int executionExitCode = ExecuteBinaryFile();
        output.Add(executionSTDOUT);
        output.Add(executionSTDERR);

        return new RuntimeResponse("clang", guid.ToString(), output);
    }

    ~ClangExecutor() {
        Cleanup();
    }
}
