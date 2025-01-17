using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class ErrorResponse
{
    public required string Message { get; set; }
    public required string Details { get; set; }
}

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
    public Task<IActionResult> ExecuteCode(string type, [FromBody] RuntimeRequest request)
    {
        ICollection<RuntimeDTO> result;
        try
        {
            result = _ExecutorStorage.GetExecutor(type).Execute(request);
        }
        catch (System.Exception e)
        {
            var errorResponse = new ErrorResponse
            {
                Message = e.Message,
                Details = e.StackTrace ?? "StackTrace is not provided"
            };
            return Task.FromResult<IActionResult>(BadRequest(errorResponse));
        }
        return Task.FromResult<IActionResult>(Ok(result));
    }
}
