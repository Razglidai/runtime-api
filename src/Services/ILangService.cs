using System.Numerics;

public interface ILangService {
    int Build(string Code, List<string> Args, Dictionary<string, string> Output);
    int Execute(Dictionary<string, string>Output);
    void Cleanup();
}