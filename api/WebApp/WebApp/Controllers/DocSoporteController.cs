using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocSoporteController : ControllerBase
    {
        public DocSoporteController() { }

        [HttpGet]
        public IActionResult Get()
        {
                return Ok("DocSoporteController is working!");
        }
    }
}
