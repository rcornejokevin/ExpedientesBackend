using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using DBHandler.Service.Catalog;
using DBHandler.Models;
using ApiHandler.Models.Catalog;
namespace ApiHandler.Controllers.Catalog
{
    [Route("flujo")]
    [ApiController]
    public class FlujoController : Controller
    {
        public Jwt jwt;
        public FlujoService flujoService;
        public FlujoController(Jwt jwt, FlujoService flujoService)
        {
            this.jwt = jwt;
            this.flujoService = flujoService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetElement([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Detalle de flujo";
            try
            {
                Flujo? flujo = await flujoService.GetByIdAsync(id);
                if (flujo == null)
                {
                    response.code = "404";
                    response.message = "Flujo no encontrado";
                    return Ok(response);
                }
                response.data = new
                {
                    id = flujo.Id,
                    nombre = flujo.Nombre,
                    correlativo = flujo.Correlativo,
                    detalle = flujo.Detalle,
                    cierreArchivado = flujo.CierreArchivado,
                    cierreDevolucionAlRemitente = flujo.CierreDevolucionAlRemitente,
                    cierreEnviadoAJudicial = flujo.CierreEnviadoAJudicial,
                    flujoAsociado = flujo.FlujoAsociado,
                    activo = flujo.Activo
                };
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpGet("list")]
        public async Task<IActionResult> List([FromHeader] string Authorization)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Listado de flujos";
            try
            {
                var flujos = await flujoService.GetAllAsync();
                response.data = flujos.Select(flujo => new
                {
                    id = flujo.Id,
                    nombre = flujo.Nombre,
                    correlativo = flujo.Correlativo,
                    detalle = flujo.Detalle,
                    cierreArchivado = flujo.CierreArchivado,
                    cierreDevolucionAlRemitente = flujo.CierreDevolucionAlRemitente,
                    cierreEnviadoAJudicial = flujo.CierreEnviadoAJudicial,
                    flujoAsociado = flujo.FlujoAsociado,
                    activo = flujo.Activo
                });
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewFlujoRequest flujoRequest)
        {

            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? "Solicitud inválida" : e.ErrorMessage)
                    .ToList();
                response.code = "400";
                response.message = errors.FirstOrDefault() ?? "Solicitud inválida";
                response.data = errors;
                return Ok(response);
            }
            Flujo flujo = new Flujo(0, flujoRequest.nombre, flujoRequest.correlativo, flujoRequest.detalle, true,
            flujoRequest.cierreArchivado, flujoRequest.cierreDevolucionAlRemitente,
            flujoRequest.cierreEnviadoAJudicial, flujoRequest.flujoAsociado);
            try
            {
                flujo = await flujoService.AddAsync(flujo);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return Ok(response);
            }

            response.code = "000";
            response.message = "Flujo creado correctamente";
            response.data = flujo;
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditFlujoRequest flujoRequest)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? "Solicitud inválida" : e.ErrorMessage)
                    .ToList();
                response.code = "400";
                response.message = errors.FirstOrDefault() ?? "Solicitud inválida";
                response.data = errors;
                return Ok(response);
            }
            Flujo? flujo = await flujoService.GetByIdAsync(flujoRequest.id);
            if (flujo == null)
            {
                response.code = "404";
                response.message = "Flujo no encontrada";
                return Ok(response);
            }
            flujo.Nombre = flujoRequest.nombre;
            flujo.Correlativo = flujoRequest.correlativo;
            flujo.Detalle = flujoRequest.detalle;
            flujo.CierreArchivado = flujoRequest.cierreArchivado;
            flujo.CierreDevolucionAlRemitente = flujoRequest.cierreDevolucionAlRemitente;
            flujo.CierreEnviadoAJudicial = flujoRequest.cierreEnviadoAJudicial;
            flujo.FlujoAsociado = flujoRequest.flujoAsociado;
            try
            {
                flujo = await flujoService.EditAsync(flujo);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return Ok(response);
            }
            response.code = "000";
            response.message = "Flujo editado correctamente";
            response.data = flujo;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            Flujo? flujo = await flujoService.GetByIdAsync(id);
            if (flujo == null)
            {
                response.code = "404";
                response.message = "Flujo no encontrada";
                return Ok(response);
            }
            flujo.Activo = false;
            try
            {
                flujo = await flujoService.EditAsync(flujo);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            response.code = "000";
            response.message = "Flujo eliminado correctamente";
            return Ok(response);
        }
    }
}