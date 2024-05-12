using Microsoft.AspNetCore.Mvc;
using WebGameLogic.Playfab;

namespace WebGameLogic
{
    [ApiController]
    [Route("cloudscript")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CloudscriptController : Controller
    {
        private ILogger _log;

        public CloudscriptController(ILogger log)
        {
            _log = log;
        }

        [HttpPost]
        [Route("execute")]
        private void Execute([FromBody]CloudscriptRequest<FunctionArgument> request)
        {
            ExecuteInternal(request.FunctionArgument);
        }

        [HttpPost]
        [Route("executeinternal")]
        private void ExecuteInternal([FromBody] FunctionArgument request)
        {
            _log.LogInformation("Starting execution");

        }
    }
}
