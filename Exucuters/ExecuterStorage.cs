using System.Reflection;

public class ExecuterStorage
{
    private readonly Dictionary<string, IRuntimeExecuter> Executers = new();
    public ExecuterStorage()
    {
        this.RegisterExecuters();
    }
    private void RegisterExecuters()
    {
        var executersTypes =
            from t in
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
            where t.GetCustomAttribute<ExecutersAttribute>() != null
                  &&
                  typeof(IRuntimeExecuter).IsAssignableFrom(t)
            select t;
        foreach (var type in executersTypes)
        {
            var attribute = type.GetCustomAttribute<ExecutersAttribute>();
            var instance = (IRuntimeExecuter)Activator.CreateInstance(type);
            Executers[attribute.Type] = instance;
        }
    }

    public IRuntimeExecuter GetExecuter(string type)
    {
        return Executers.TryGetValue(type, out var executers) ? executers : throw new Exception("no runtime");
    }
    public ICollection<string> GetExecutersList()
    {
        return Executers.Keys;
    }
}