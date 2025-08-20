using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using DBHandler.Service.Catalog;
using DBHandler.Models;
using ApiHandler.Models.Catalog;
using BusinessLogic.Services;
namespace ApiHandler.Controllers.Catalog
{
    [Route("campo")]
    [ApiController]
    public class CampoController : Controller
    {
        public Jwt jwt;
        public CampoService campoService;
        public EtapaService etapaService;
        public EtapaDetalleService etapaDetalleService;
        public CampoLogic campoLogic;


        public CampoController(Jwt jwt, CampoService campoService,
         EtapaService etapaService, EtapaDetalleService etapaDetalleService, CampoLogic campoLogic)
        {
            this.jwt = jwt;
            this.campoService = campoService;
            this.etapaService = etapaService;
            this.etapaDetalleService = etapaDetalleService;
            this.campoLogic = campoLogic;
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
            response.message = "Detalle de campo";
            try
            {
                Campo? campo = await campoService.GetByIdAsync(id);
                if (campo == null)
                {
                    response.code = "404";
                    response.message = "Campo no encontrado";
                    return NotFound(response);
                }
                response.data = campo;
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
            response.message = "Listado de campos";
            try
            {
                response.data = await campoService.GetAllAsync();
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
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewCampoRequest campoRequest)
        {

            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }

            ResponseApi response = new ResponseApi();
            Campo campo = new Campo();
            campo.Nombre = campoRequest.nombre;
            campo.EtapaId = campoRequest.etapaId == 0 ? null : campoRequest.etapaId;
            campo.EtapaDetalleId = campoRequest.subEtapaId == 0 ? null : campoRequest.subEtapaId;
            campo.Tipo = campoRequest.tipoCampo;
            campo.Orden = campoRequest.orden;
            campo.Requerido = campoRequest.requerido;
            try
            {
                await campoLogic.addCampo(campo);
            }
            catch (KeyNotFoundException ex)
            {
                response.code = "404";
                response.message = ex.Message;
                return NotFound(response);
            }
            catch (ArgumentException ex)
            {
                response.code = "400";
                response.message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Campo creado correctamente";
            response.data = campo;
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditCampoRequest campoRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            Campo? campo = await campoService.GetByIdAsync(campoRequest.id);
            if (campo == null)
            {
                response.code = "404";
                response.message = "Campo no encontrada";
                return NotFound(response);
            }
            campo.Nombre = campoRequest.nombre;
            campo.EtapaId = campoRequest.etapaId;
            campo.EtapaDetalleId = campoRequest.subEtapaId;
            campo.Tipo = campoRequest.tipoCampo;
            campo.Orden = campoRequest.orden;
            campo.Requerido = campoRequest.requerido;
            try
            {
                campo = await campoLogic.editCampo(campo);
            }
            catch (KeyNotFoundException ex)
            {
                response.code = "404";
                response.message = ex.Message;
                return NotFound(response);
            }
            catch (ArgumentException ex)
            {
                response.code = "400";
                response.message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Campo editado correctamente";
            response.data = campo;
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
            Campo? campo = await campoService.GetByIdAsync(id);
            if (campo == null)
            {
                response.code = "404";
                response.message = "Campo no encontrada";
                return NotFound(response);
            }
            campo.Activo = false;
            try
            {
                campo = await campoService.EditAsync(campo);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            response.code = "000";
            response.message = "Campo eliminado correctamente";
            return Ok(response);
        }

    }
}