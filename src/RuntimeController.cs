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
    public async Task<IActionResult> GetRuntimeList()
    {
        return Ok(_ExecutorStorage.GetExecutorsList());
    }
    [Route("{type}")]
    [HttpPost]
    public async Task<IActionResult> ExecuteCode(string type, [FromBody]RuntimeRequest request)
    {
        RuntimeResponse result;
        try
        {
            result = _ExecutorStorage.GetExecutor(type).Execute(request);
        }
        catch (System.Exception e)
        {
            return BadRequest(e);
        }
        return Ok(result);
    }
}