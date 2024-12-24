using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RuntimeController : ControllerBase
{
    private ExecuterStorage _executerStorage;

    public RuntimeController(ExecuterStorage executerStorage)
    {
        _executerStorage = executerStorage;
    }

    [HttpGet]
    public async Task<IActionResult> GetRuntimeList()
    {
        return Ok(_executerStorage.GetExecutersList());
    }
    [Route("{type}")]
    [HttpPost]
    public async Task<IActionResult> ExecuteCode(string type, [FromBody]RuntimeRequest request)
    {
        RuntimeResponse result;
        try
        {
            result = _executerStorage.GetExecuter(type).Execute(request);
        }
        catch (System.Exception e)
        {
            return BadRequest(e);
        }
        return Ok(result);
    }
}