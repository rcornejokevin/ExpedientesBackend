using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using ApiHandler.Models.Cases;
using BusinessLogic.Services;
using DBHandler.Models;
namespace ApiHandler.Controllers.Catalog
{
    [Route("cases")]
    [ApiController]
    public class CasesController : Controller
    {
        public Jwt jwt;
        public CasesLogic casesLogic;
        public CasesController(Jwt jwt, CasesLogic casesLogic)
        {
            this.jwt = jwt;
            this.casesLogic = casesLogic;
        }
        [HttpPost("add")]
        public async Task<IActionResult> addElement([FromHeader] string Authorization, [FromBody] NewCasesRequest casesRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Detalle de campo";
            Expediente expediente = new Expediente();
            expediente.EtapaId = casesRequest.etapaId;
            //await casesLogic.newCases(casesRequest.expediente);
            return Ok(response);
        }
    }
}