[AttributeUsage(AttributeTargets.Class)]
public class ExecutersAttribute : System.Attribute
{
    public ExecutersAttribute(string type)
    {
        Type = type;
    }

    public string Type { get; }
}