using ResponseApi = ApiHandler.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ApiHandler.Models.Security;
using DBHandler.Models;
using DBHandler.Service.Catalog;
namespace ApiHandler.Controllers.Catalog
{
    [Route("remitente")]
    [ApiController]
    public class RemitenteController : Controller
    {
        private readonly RemitenteService remitenteService;
        private readonly Jwt jwt;
        public RemitenteController(RemitenteService remitenteService, Jwt jwt)
        {
            this.remitenteService = remitenteService;
            this.jwt = jwt;
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
            response.message = "Listado de remitentes";
            try
            {
                response.data = await remitenteService.GetAllAsync();
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
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] AddRemitenteRequest remitenteRequest)
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
            response.code = "000";
            response.message = "Remitente creado correctamente";
            try
            {
                Remitente remitente = new Remitente(0, remitenteRequest.descripcion, true);
                response.data = await remitenteService.createAsync(remitente);
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
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditRemitenteRequest remitenteRequest)
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
            response.code = "000";
            response.message = "Edición de remitente";
            try
            {
                Remitente? remitente = await remitenteService.getRemitenteById(remitenteRequest.id);
                if (remitente == null)
                {
                    response.code = "404";
                    response.message = "Remitente no encontrado";
                    return Ok(response);
                }
                remitente.Descripcion = remitenteRequest.descripcion ?? "";
                try
                {
                    remitente = await remitenteService.updateAsync(remitente);
                    response.data = remitente;
                }
                catch (Exception ex)
                {
                    response.code = "500";
                    response.message = ex.Message;
                    response.data = ex.StackTrace;
                    return Ok(response);
                }
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
            response.code = "000";
            response.message = "Remitente eliminado correctamente";
            try
            {
                Remitente? remitente = await remitenteService.getRemitenteById(id);
                if (remitente == null)
                {
                    response.code = "404";
                    response.message = "Remitente no encontrado";
                    return Ok(response);
                }
                remitente.Activo = false;
                await remitenteService.updateAsync(remitente);
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
            response.message = "Detalle de remitente";
            try
            {
                Remitente? remitente = await remitenteService.getRemitenteById(id);
                if (remitente == null)
                {
                    response.code = "404";
                    response.message = "Remitente no encontrado";
                    return Ok(response);
                }
                response.data = remitente;
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
    }
}