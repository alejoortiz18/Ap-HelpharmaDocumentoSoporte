using Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocSoporteController : ControllerBase
    {
        private readonly IDocSoportBusiness _docSoport;

        public DocSoporteController(IDocSoportBusiness doc) {
        _docSoport = doc;
        }


        [HttpGet]
        public IActionResult Get()
        {
            var result = _docSoport.GetSoporte("FMF193083").Result;   

            return Ok(result);
        }
    }
}
