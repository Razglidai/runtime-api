[ExecutorsAttribute("lua")]

public class LuaExecutor : IRuntimeExecutor
{
    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return InterpretRunner.Run((new RunnerData(LangRunnerConstants.LUA_INTERPRETER, "%f", ".lua")), request);
    }
}
