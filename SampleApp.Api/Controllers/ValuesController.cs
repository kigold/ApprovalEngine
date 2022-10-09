using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok(new[] { "Red", "Green", "Blue" });
        }
    }
}
