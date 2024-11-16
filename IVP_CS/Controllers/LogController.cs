using CS_Console.LogRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IVP_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        ILog _log;
        public LogController(ILog log)
        {
            _log = log;
        }
        [HttpGet("equitylogs")]
        public IActionResult GetLogs()
        {
            var result = _log.GetAllSecurityUpdateLogs();
            return Ok(result);
        }

        [HttpGet("bondlogs")]
        public IActionResult GetBondLogs()
        {
            var res = _log.GetAllBondsUpdateLogs();
            return Ok(res);
        }
    }
}
