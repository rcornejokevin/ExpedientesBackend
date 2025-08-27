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
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Listado de etapas";
            try
            {
                response.data = await etapaService.GetAllAsync();
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
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewEtapaRequest etapaRequest)
        {

            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
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
                return BadRequest(badResponse);
            }
            List<EtapaDb> listEtapaByFlujoId = await etapaService.GetAllByFlujoIdAsync(etapaRequest.flujoId);
            ResponseApi response = new ResponseApi();
            EtapaDb etapa = new EtapaDb();
            etapa.Nombre = etapaRequest.nombre;
            etapa.Activo = true;
            etapa.Orden = listEtapaByFlujoId.Count() + 1;
            etapa.Detalle = etapaRequest.detalle;
            etapa.FlujoId = etapaRequest.flujoId;
            try
            {
                etapa = await etapaService.AddAsync(etapa);
            }
            catch (NotFoundException ex)
            {
                response.code = "404";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }

            response.code = "000";
            response.message = "Etapa creada correctamente";
            response.data = etapa;
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditEtapaRequest etapaRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
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
                return BadRequest(badResponse);
            }
            ResponseApi response = new ResponseApi();
            EtapaDb? etapa = await etapaService.GetEtapaByIdAsync(etapaRequest.id);
            if (etapa == null)
            {
                response.code = "404";
                response.message = "Etapa no encontrada";
                return NotFound(response);
            }
            etapa.Nombre = etapaRequest.nombre;
            etapa.Detalle = etapaRequest.detalle;
            etapa.FlujoId = etapaRequest.flujoId;
            try
            {
                etapa = await etapaService.EditAsync(etapa);
            }
            catch (NotFoundException ex)
            {
                response.code = "404";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Etapa editada correctamente";
            response.data = etapa;
            return Ok(response);
        }
        [HttpPut("orden")]
        public async Task<IActionResult> Orden([FromHeader] string Authorization, [FromBody] EditOrdenEtapaRequest etapaRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization)) return Unauthorized();

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
                return BadRequest(badResponse);
            }

            ResponseApi response = new ResponseApi();
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
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            EtapaDb? etapa = await etapaService.GetEtapaByIdAsync(id);
            if (etapa == null)
            {
                response.code = "404";
                response.message = "Etapa no encontrada";
                return NotFound(response);
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
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Etapa eliminada correctamente";
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
            response.message = "Detalle de etapa";
            try
            {
                EtapaDb? etapa = await etapaService.GetEtapaByIdAsync(id);
                if (etapa == null)
                {
                    response.code = "404";
                    response.message = "Etapa no encontrada";
                    return NotFound(response);
                }
                response.data = etapa;
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