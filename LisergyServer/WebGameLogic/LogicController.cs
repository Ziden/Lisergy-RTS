using Microsoft.AspNetCore.Mvc;
using WebGameLogic.Playfab;

namespace WebGameLogic;

[ApiController]
[Route("cloudscript")]
[Produces("application/json")]
[Consumes("application/json")]
public class CloudscriptController : Controller
{
	private readonly ILogger<CloudscriptController> _log;

	public CloudscriptController(ILogger<CloudscriptController> log)
	{
		_log = log;
	}

	[HttpPost]
	[Route("execute")]
	public IActionResult Execute([FromBody] CloudscriptRequest<FunctionArgument> request)
	{
		return ExecuteInternal(request.FunctionArgument);
	}

	[HttpPost]
	[Route("executeinternal")]
	public IActionResult ExecuteInternal([FromBody] FunctionArgument request)
	{
		_log.LogInformation("Starting execution");
		return Ok("Execution completed successfully");
	}
}