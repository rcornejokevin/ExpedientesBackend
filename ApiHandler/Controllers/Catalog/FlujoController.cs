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
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Detalle de flujo";
            try
            {
                Flujo? flujo = await flujoService.GetByIdAsync(id);
                if (flujo == null)
                {
                    response.code = "404";
                    response.message = "Flujo no encontrado";
                    return NotFound(response);
                }
                response.data = flujo;
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
        [HttpGet("list")]
        public async Task<IActionResult> List([FromHeader] string Authorization)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Listado de flujos";
            try
            {
                response.data = await flujoService.GetAllAsync();
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
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewFlujoRequest flujoRequest)
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
            Flujo flujo = new Flujo(0, flujoRequest.nombre, flujoRequest.detalle, true);
            try
            {
                flujo = await flujoService.AddAsync(flujo);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return StatusCode(500, response);
            }

            response.code = "000";
            response.message = "Flujo creado correctamente";
            response.data = flujo;
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditFlujoRequest flujoRequest)
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
            Flujo? flujo = await flujoService.GetByIdAsync(flujoRequest.id);
            if (flujo == null)
            {
                response.code = "404";
                response.message = "Flujo no encontrada";
                return NotFound(response);
            }
            flujo.Nombre = flujoRequest.nombre;
            flujo.Detalle = flujoRequest.detalle;
            try
            {
                flujo = await flujoService.EditAsync(flujo);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Flujo editado correctamente";
            response.data = flujo;
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
            Flujo? flujo = await flujoService.GetByIdAsync(id);
            if (flujo == null)
            {
                response.code = "404";
                response.message = "Flujo no encontrada";
                return NotFound(response);
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
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Flujo eliminado correctamente";
            return Ok(response);
        }
    }
}