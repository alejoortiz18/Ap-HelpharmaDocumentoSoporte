using Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Request;

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


        [HttpPost("soportes/by-dctoprv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSoportesByDCTOPRV([FromBody] string request)
        {
            if (string.IsNullOrWhiteSpace(request))
                return BadRequest("El campo Dctoprv es obligatorio.");

            var result = await _docSoport.GetSoporte(request);

            return Ok(result);
        }


        [HttpPost("soportes/Trade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSoportesTrade([FromBody] TradeDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Los datos son obligatorios.");

            var result = await _docSoport.GetSoporteTrade(request);

            return Ok(result);
        }
    }
}
