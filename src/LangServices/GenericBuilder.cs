
public class GenericBuilder
{
    public string sourceFileName = "";
    public string binaryFileName = "";

    public string generateFilename()
    {
        Random rand = new();
        return Path.Combine(RuntimeConstants.ClangDirectory, rand.Next(0, 1000000000).ToString());

    }

    public virtual int Build(string Code, string compileArgs, Dictionary<string, string> Output)
    {
        throw new NotImplementedException();
    }

    public virtual int Execute(string ExecArgs, Dictionary<string, string> Output)
    {
        throw new NotImplementedException();
    }

    public virtual void Cleanup()
    {
        if (File.Exists(sourceFileName))
        {
            File.Delete(sourceFileName);
        }
        if (File.Exists(binaryFileName))
        {
            File.Delete(binaryFileName);
        }
    }

}