using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cagnaz.Family.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()=>Ok(DateTime.Now);
    }
}
