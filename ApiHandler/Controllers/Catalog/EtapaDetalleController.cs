using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using DBHandler.Service.Catalog;
using ApiHandler.Models.Catalog;
using EtapaDb = DBHandler.Models.Etapa;
using DBHandler.Models;


namespace ApiHandler.Controllers.Catalog
{
    [Route("etapa_detalle")]
    [ApiController]
    public class EtapaDetalleController : ControllerBase
    {
        private readonly Jwt jwt;
        private readonly EtapaDetalleService etapaDetalleService;
        public EtapaDetalleController(Jwt _jwt, EtapaDetalleService _etapaDetalleService)
        {
            jwt = _jwt;
            etapaDetalleService = _etapaDetalleService;
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
            response.message = "Listado de etapas";
            try
            {
                response.data = await etapaDetalleService.GetAllAsync();
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
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewEtapaDetalleRequest etapaDetalleRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Etapa creada correctamente";
            EtapaDetalle etapaDetalle = new EtapaDetalle();
            etapaDetalle.EtapaId = etapaDetalleRequest.etapaId;
            etapaDetalle.Activo = true;
            etapaDetalle.Orden = etapaDetalleRequest.orden;
            etapaDetalle.Nombre = etapaDetalleRequest.nombre;
            etapaDetalle.Detalle = etapaDetalleRequest.detalle;
            try
            {
                response.data = await etapaDetalleService.AddAsync(etapaDetalle);
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
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditEtapaDetalleRequest etapaDetalleRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Etapa editada correctamente";
            EtapaDetalle? etapaDetalle = await etapaDetalleService.GetByIdAsync(etapaDetalleRequest.id);
            if (etapaDetalle == null)
            {
                response.code = "404";
                response.message = "Etapa no encontrada";
                return NotFound(response);
            }
            etapaDetalle.EtapaId = etapaDetalleRequest.etapaId;
            etapaDetalle.Activo = true;
            etapaDetalle.Orden = etapaDetalleRequest.orden;
            etapaDetalle.Nombre = etapaDetalleRequest.nombre;
            etapaDetalle.Detalle = etapaDetalleRequest.detalle;
            try
            {
                response.data = await etapaDetalleService.EditAsync(etapaDetalle);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromHeader] string Authorization, int id)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            EtapaDetalle? etapaDetalle = await etapaDetalleService.GetByIdAsync(id);
            if (etapaDetalle == null)
            {
                response.code = "404";
                response.message = "Flujo no encontrada";
                return NotFound(response);
            }
            etapaDetalle.Activo = false;
            try
            {
                await etapaDetalleService.EditAsync(etapaDetalle);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Etapa Detalle eliminado correctamente";
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetElement([FromHeader] string Authorization, int id)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Detalle de detalle etapa";
            try
            {
                EtapaDetalle? etapaDetalle = await etapaDetalleService.GetByIdAsync(id);
                if (etapaDetalle == null)
                {
                    response.code = "404";
                    response.message = "Etapa detalle no encontrada";
                    return NotFound(response);
                }
                response.data = etapaDetalle;
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