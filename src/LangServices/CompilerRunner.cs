
using System.Diagnostics;
using IronPython.Runtime.Operations;

public class CompilerRunner
{
    string sourceFilePath, binaryFilePath;
    string compilerPath, buildPrompt;
    public CompilerRunner(string compilerPath, string buildPrompt)
    {
        this.compilerPath = compilerPath;
        this.buildPrompt = buildPrompt;
    }

    string GenerateRandomString(uint length = 16)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, (int)length).Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    public List<Triplet<string, string, int>> Run(string code, ICollection<string> input)
    {
        List<Triplet<string, string, int>> outputs = new List<Triplet<string, string, int>>();

        string filenameBase = GenerateRandomString();
        sourceFilePath = Path.Combine(LangServiceConstants.POOL_DIR, filenameBase + ".src");
        binaryFilePath = Path.Combine(LangServiceConstants.POOL_DIR, filenameBase + ".exe");

        File.WriteAllText(sourceFilePath, code); 

        Triplet<string, string, int> compileOut = CompileSource();
        if(compileOut.third != 0) {
            outputs.Add(compileOut);
            return outputs;
        }

        foreach(string stdin in input) {
            Triplet<string, string, int> output = Execute();
            outputs.Add(output);
        }
        return outputs;
    }

    private Triplet<string, string, int> Execute()
    {
        Triplet<string, string, int> output = new Triplet<string, string, int>("", "", 0);
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "gcc";
            process.StartInfo.Arguments = buildPrompt.Replace("%s", sourceFilePath).Replace("%b", binaryFilePath);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
            
            output.first = process.StandardOutput.ReadToEnd();
            output.second = process.StandardError.ReadToEnd();
            process.WaitForExit();
            output.third = process.ExitCode;
            Console.WriteLine("Процесс завершился с кодом: " + output.third);
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при компиляции: " + e.Message);
        }
        return output;
    }

    private Triplet<string, string, int> CompileSource()
    {
        Triplet<string, string, int> output = new Triplet<string, string, int>("", "", -1);
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = compilerPath;
            process.StartInfo.Arguments = $"-o {binaryFilePath} {sourceFilePath}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
            
            output.first = process.StandardOutput.ReadToEnd();
            output.second = process.StandardError.ReadToEnd();
            process.WaitForExit();
            output.third = process.ExitCode;
            Console.WriteLine("Процесс завершился с кодом: " + output.third);
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при компиляции: " + e.Message);
        }
        return output;
    }

    public void Cleanup()
    {
        if (File.Exists(sourceFilePath))
            File.Delete(sourceFilePath);
        if (File.Exists(binaryFilePath))
            File.Delete(binaryFilePath);
    }

    ~CompilerRunner()
    {
        Cleanup();
    }
}