public class GenericInterpreter
{
    public string sourceFileName = "";

    public string generateFilename()
    {
        Random rand = new();
        return Path.Combine(RuntimeConstants.ClangDirectory, rand.Next(0, 1000000000).ToString());

    }

    public virtual int Execute(string Code, Dictionary<string, string> Output)
    {
        throw new NotImplementedException();
    }

    public virtual void Cleanup()
    {
        if(File.Exists(sourceFileName)) {
            File.Delete(sourceFileName);
        }
    }

}