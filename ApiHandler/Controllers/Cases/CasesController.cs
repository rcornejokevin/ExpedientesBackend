using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using ApiHandler.Models.Cases;
using BusinessLogic.Services;
using DBHandler.Models;
using DBHandler.Service.Catalog;
namespace ApiHandler.Controllers.Catalog
{
    [Route("cases")]
    [ApiController]
    public class CasesController : Controller
    {
        public Jwt jwt;
        public CasesLogic casesLogic;
        public CasesService casesService;
        public CasesController(Jwt jwt, CasesLogic casesLogic, CasesService CasesService)
        {
            this.jwt = jwt;
            this.casesLogic = casesLogic;
            this.casesService = CasesService;
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
            expediente.EtapaDetalleId = casesRequest.subEtapaId == 0 ? null : casesRequest.subEtapaId;
            expediente.Codigo = casesRequest.codigo;
            expediente.Nombre = casesRequest.nombre;
            expediente.Ubicacion = "";
            expediente.NombreArchivo = casesRequest.nombreArchivo;
            expediente.FechaIngreso = casesRequest.fechaIngreso;
            expediente.FechaActualizacion = DateTime.UtcNow;
            expediente.AsesorId = casesRequest.asesor;
            expediente.Estatus = "Abierto";
            expediente.Activo = true;
            expediente.CampoValorJson = casesRequest.campos;
            await casesLogic.newCases(expediente, casesRequest.archivo);
            return Ok(response);
        }
        [HttpGet("list")]
        public async Task<IActionResult> List([FromHeader] string Authorization)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Listado de Expedientes";
            try
            {
                response.data = await casesService.GetAllAsync();
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            return Ok(response);
        }
    }
}
