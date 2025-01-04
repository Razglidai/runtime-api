using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RuntimeController : ControllerBase
{
    private ExecutorStorage _ExecutorStorage;

    public RuntimeController(ExecutorStorage ExecutorStorage)
    {
        _ExecutorStorage = ExecutorStorage;
    }

    [HttpGet]
    public Task<IActionResult> GetRuntimeList()
    {
        return Task.FromResult<IActionResult>(Ok(_ExecutorStorage.GetExecutorsList()));
    }
    [Route("{type}")]
    [HttpPost]
    public Task<IActionResult> ExecuteCode(string type, [FromBody]RuntimeRequest request)
    {
        RuntimeResponse result;
        try
        {
            result = _ExecutorStorage.GetExecutor(type).Execute(request);
        }
        catch (System.Exception e)
        {
            return Task.FromResult<IActionResult>(BadRequest(e));
        }
        return Task.FromResult<IActionResult>(Ok(result));
    }
}