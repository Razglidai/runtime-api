using System;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutorsAttribute : System.Attribute
{
    public ExecutorsAttribute(string type)
    {
        Type = type;
    }

    public string Type { get; }
}