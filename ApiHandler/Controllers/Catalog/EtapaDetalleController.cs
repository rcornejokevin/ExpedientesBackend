using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using DBHandler.Service.Catalog;
using ApiHandler.Models.Catalog;
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
                response.data = await etapaDetalleService.GetAllAsync();
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
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewEtapaDetalleRequest etapaDetalleRequest)
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
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditEtapaDetalleRequest etapaDetalleRequest)
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
            response.message = "Etapa editada correctamente";
            EtapaDetalle? etapaDetalle = await etapaDetalleService.GetByIdAsync(etapaDetalleRequest.id);
            if (etapaDetalle == null)
            {
                response.code = "404";
                response.message = "Etapa no encontrada";
                return Ok(response);
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
                return Ok(response);
            }
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
                var validationErrors = ModelState
                    .Where(kvp => kvp.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var badResponse = new ResponseApi
                {
                    code = "400",
                    message = "Error de validación de campo",
                    data = validationErrors
                };
                return Ok(badResponse);
            }
            try
            {
                foreach (var item in etapaRequest.items)
                {
                    EtapaDetalle? etapaDetalle = await etapaDetalleService.GetByIdAsync(item.id);
                    if (etapaDetalle != null)
                    {
                        etapaDetalle.Orden = item.orden;
                        await etapaDetalleService.EditAsync(etapaDetalle);
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }

            response.code = "000";
            response.message = "SubEtapa ordenada correctamente";
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
            EtapaDetalle? etapaDetalle = await etapaDetalleService.GetByIdAsync(id);
            if (etapaDetalle == null)
            {
                response.code = "404";
                response.message = "Flujo no encontrada";
                return Ok(response);
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
                return Ok(response);
            }
            response.code = "000";
            response.message = "Etapa Detalle eliminado correctamente";
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
            response.message = "Detalle de detalle etapa";
            try
            {
                EtapaDetalle? etapaDetalle = await etapaDetalleService.GetByIdAsync(id);
                if (etapaDetalle == null)
                {
                    response.code = "404";
                    response.message = "Etapa detalle no encontrada";
                    return Ok(response);
                }
                response.data = etapaDetalle;
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