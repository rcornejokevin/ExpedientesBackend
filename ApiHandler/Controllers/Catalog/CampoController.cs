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
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Detalle de campo";
            try
            {
                Campo? campo = await campoService.GetByIdAsync(id);
                if (campo == null)
                {
                    response.code = "404";
                    response.message = "Campo no encontrado";
                    return Ok(response);
                }
                response.data = new
                {
                    id = campo.Id,
                    nombre = campo.Nombre,
                    label = campo.Label,
                    placeHolder = campo.Placeholder,
                    opciones = campo.Opciones,
                    orden = campo.Orden,
                    tipoCampo = campo.Tipo,
                    flujoId = campo.FlujoId,
                    etapaId = campo.EtapaId,
                    requerido = campo.Requerido,
                    editable = campo.Editable,
                    activo = campo.Activo
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
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewCampoRequest campoRequest)
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
            Campo campo = new Campo();
            campo.Nombre = campoRequest.nombre;
            campo.EtapaId = campoRequest.etapaId == 0 ? null : campoRequest.etapaId;
            campo.FlujoId = campoRequest.flujoId;
            campo.Tipo = campoRequest.tipoCampo;
            campo.Orden = campoRequest.orden;
            campo.Requerido = campoRequest.requerido;
            campo.Label = campoRequest.label;
            campo.Placeholder = campoRequest.placeHolder;
            campo.Opciones = campoRequest.tipoCampo == "Opciones" ? campoRequest.opciones : "";
            campo.Editable = campoRequest.editable;
            try
            {
                await campoLogic.addCampo(campo);
            }
            catch (KeyNotFoundException ex)
            {
                response.code = "404";
                response.message = ex.Message;
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                response.code = "400";
                response.message = ex.Message;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return Ok(response);
            }
            response.code = "000";
            response.message = "Campo creado correctamente";
            response.data = campo;
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditCampoRequest campoRequest)
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
            Campo? campo = await campoService.GetByIdAsync(campoRequest.id);
            if (campo == null)
            {
                response.code = "404";
                response.message = "Campo no encontrada";
                return Ok(response);
            }
            campo.Nombre = campoRequest.nombre;
            campo.EtapaId = campoRequest.etapaId == 0 ? null : campoRequest.etapaId;
            campo.FlujoId = campoRequest.flujoId;
            campo.Tipo = campoRequest.tipoCampo;
            campo.Orden = campoRequest.orden;
            campo.Requerido = campoRequest.requerido;
            campo.Label = campoRequest.label;
            campo.Placeholder = campoRequest.placeHolder;
            campo.Opciones = campoRequest.tipoCampo == "Opciones" ? campoRequest.opciones : "";
            campo.Editable = campoRequest.editable;
            try
            {
                campo = await campoLogic.editCampo(campo);
            }
            catch (KeyNotFoundException ex)
            {
                response.code = "404";
                response.message = ex.Message;
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                response.code = "400";
                response.message = ex.Message;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return Ok(response);
            }
            response.code = "000";
            response.message = "Campo editado correctamente";
            response.data = campo;
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

            try
            {
                foreach (var item in etapaRequest.items)
                {
                    Campo? campo = await campoService.GetByIdAsync(item.id);
                    if (campo != null)
                    {
                        campo.Orden = item.orden;
                        await campoService.EditAsync(campo);
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
            response.message = "Campo ordenada correctamente";
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
            Campo? campo = await campoService.GetByIdAsync(id);
            if (campo == null)
            {
                response.code = "404";
                response.message = "Campo no encontrada";
                return Ok(response);
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
                return Ok(response);
            }
            response.code = "000";
            response.message = "Campo eliminado correctamente";
            return Ok(response);
        }

    }
}