using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using DBHandler.Service.Catalog;
using ApiHandler.Models.Catalog;
using EtapaDb = DBHandler.Models.Etapa;
using DBHandler.Exceptions;
namespace ApiHandler.Controllers.Catalog
{
    [Route("etapa")]
    [ApiController]
    public class EtapaController : ControllerBase
    {
        private readonly Jwt jwt;
        private readonly EtapaService etapaService;
        private readonly FlujoService flujoService;
        public EtapaController(Jwt _jwt, EtapaService etapaService, FlujoService flujoService)
        {
            jwt = _jwt;
            this.etapaService = etapaService;
            this.flujoService = flujoService;
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
            response.message = "Listado de etapas";
            try
            {
                var etapas = await etapaService.GetAllAsync();
                response.data = etapas.Select(e => new
                {
                    e.Id,
                    e.Nombre,
                    e.Orden,
                    e.Detalle,
                    e.FlujoId,
                    e.FinDeFlujo,
                    e.Activo
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
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewEtapaRequest etapaRequest)
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
            List<EtapaDb> listEtapaByFlujoId = await etapaService.GetAllByFlujoIdAsync(etapaRequest.flujoId);
            EtapaDb etapa = new EtapaDb();
            etapa.Nombre = etapaRequest.nombre;
            etapa.Activo = true;
            etapa.Orden = listEtapaByFlujoId.Count() + 1;
            etapa.Detalle = etapaRequest.detalle;
            etapa.FlujoId = etapaRequest.flujoId;
            etapa.FinDeFlujo = etapaRequest.finDeFlujo;
            try
            {
                etapa = await etapaService.AddAsync(etapa);
            }
            catch (NotFoundException ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }

            response.code = "000";
            response.message = "Etapa creada correctamente";
            response.data = etapa;
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditEtapaRequest etapaRequest)
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
            EtapaDb? etapa = await etapaService.GetEtapaByIdAsync(etapaRequest.id);
            if (etapa == null)
            {
                response.code = "404";
                response.message = "Etapa no encontrada";
                return Ok(response);
            }
            etapa.Nombre = etapaRequest.nombre;
            etapa.Detalle = etapaRequest.detalle;
            etapa.FlujoId = etapaRequest.flujoId;
            etapa.FinDeFlujo = etapaRequest.finDeFlujo;
            try
            {
                etapa = await etapaService.EditAsync(etapa);
            }
            catch (NotFoundException ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            response.code = "000";
            response.message = "Etapa editada correctamente";
            response.data = etapa;
            return Ok(response);
        }
        [HttpPut("orden")]
        public async Task<IActionResult> Orden([FromHeader] string Authorization, [FromBody] EditOrdenEtapaRequest etapaRequest)
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
            foreach (var item in etapaRequest.items)
            {
                EtapaDb? etapa = await etapaService.GetEtapaByIdAsync(item.id);
                if (etapa != null)
                {
                    etapa.Orden = item.orden;
                    await etapaService.EditAsync(etapa);
                }
            }

            response.code = "000";
            response.message = "Etapa ordenada correctamente";
            response.data = new { };
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
            EtapaDb? etapa = await etapaService.GetEtapaByIdAsync(id);
            if (etapa == null)
            {
                response.code = "404";
                response.message = "Etapa no encontrada";
                return Ok(response);
            }
            etapa.Activo = false;
            try
            {
                etapa = await etapaService.EditAsync(etapa);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            response.code = "000";
            response.message = "Etapa eliminada correctamente";
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
            response.message = "Detalle de etapa";
            try
            {
                EtapaDb? etapa = await etapaService.GetEtapaByIdAsync(id);
                if (etapa == null)
                {
                    response.code = "404";
                    response.message = "Etapa no encontrada";
                    return Ok(response);
                }
                response.data = new
                {
                    etapa.Id,
                    etapa.Nombre,
                    etapa.Orden,
                    etapa.Detalle,
                    etapa.FlujoId,
                    etapa.FinDeFlujo,
                    etapa.Activo
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
    }
}