using System.Reflection;

public class ExecutorStorage
{
    private readonly Dictionary<string, IRuntimeExecutor> Executors = new();
    public ExecutorStorage()
    {
        this.RegisterExecutors();
    }
    private void RegisterExecutors()
    {
        var ExecutorsTypes =
            from t in
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
            where t.GetCustomAttribute<ExecutorsAttribute>() != null
                  &&
                  typeof(IRuntimeExecutor).IsAssignableFrom(t)
            select t;
        foreach (var type in ExecutorsTypes)
        {
            var attribute = type.GetCustomAttribute<ExecutorsAttribute>();
            var instance = (IRuntimeExecutor)Activator.CreateInstance(type);
            Executors[attribute.Type] = instance;
        }
    }

    public IRuntimeExecutor GetExecutor(string type)
    {
        return Executors.TryGetValue(type, out var Executors) ? Executors : throw new Exception("no runtime");
    }
    public ICollection<string> GetExecutorsList()
    {
        return Executors.Keys;
    }
}