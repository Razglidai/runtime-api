using System.Numerics;

public interface ILangService {
    int Build(string Code, string compileArgs, Dictionary<string, string> Output);
    int Execute(string ExecArgs, Dictionary<string, string>Output);
    void Cleanup();
}