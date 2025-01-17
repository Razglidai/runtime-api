public interface IRuntimeExecutor
{
    ICollection<RuntimeDTO> Execute(RuntimeRequest request);
}