using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using ApiHandler.Models.Cases;
using BusinessLogic.Services;
using DBHandler.Models;
using DBHandler.Service.Catalog;
using ApiHandler.Utils;
using BusinessLogic.Models;
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
        [HttpPut("edit")]
        public async Task<IActionResult> editElement([FromHeader] string Authorization, [FromBody] EditCasesRequest casesRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Detalle de campo";
            Expediente? expedienteOld = await casesService.GetByIdAsync(casesRequest.id);
            if (expedienteOld == null)
            {
                response.code = "404";
                response.message = "Expediente no encontrado";
                return NotFound(response);
            }
            FollowCase followCase = new FollowCase
            {
                etapaId = casesRequest.etapaId,
                subEtapaId = casesRequest.subEtapaId,
                adjuntarArchivo = casesRequest.adjuntarArchivo,
                nombreArchivo = casesRequest.nombreArchivo,
                archivo = casesRequest.archivo,
                asesor = casesRequest.asesor,
            };
            try
            {
                await casesLogic.followCase(expedienteOld, followCase);
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
            response.message = "Expediente editado correctamente";
            response.data = expedienteOld;
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
                var expedientes = await casesService.GetAllAsync();
                response.data = expedientes.Select(e => new
                {
                    e.Id,
                    e.Codigo,
                    e.Nombre,
                    e.Estatus,
                    e.FechaIngreso,
                    e.FechaActualizacion,
                    e.Activo,
                    e.Ubicacion,
                    e.CampoValorJson,
                    e.NombreArchivo,
                    e.NombreArchivoHash,
                    e.EtapaId,
                    e.EtapaDetalleId,
                    e.AsesorId,
                    flujoId = e.Etapa != null ? e.Etapa.FlujoId : 0,
                    flujo = e.Etapa?.Flujo.Nombre,
                    etapa = e.Etapa?.Nombre,
                    etapaDetalle = e.EtapaDetalle?.Nombre,
                    asesor = e.Usuario != null ? $"{e.Usuario.Username}" : "",
                }).ToList();
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
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromHeader] string Authorization, int id)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Expediente";
            try
            {
                Expediente? expediente = await casesService.GetByIdAsync(id);
                if (expediente != null)
                {
                    response.data = new
                    {
                        expediente.Id,
                        expediente.Codigo,
                        expediente.Nombre,
                        expediente.Estatus,
                        expediente.FechaIngreso,
                        expediente.FechaActualizacion,
                        expediente.Activo,
                        expediente.Ubicacion,
                        expediente.CampoValorJson,
                        expediente.NombreArchivo,
                        expediente.NombreArchivoHash,
                        expediente.EtapaId,
                        expediente.EtapaDetalleId,
                        expediente.AsesorId,
                        flujoId = expediente.Etapa != null ? expediente.Etapa.FlujoId : 0,
                        flujo = expediente.Etapa?.Flujo.Nombre,
                        etapa = expediente.Etapa?.Nombre,
                        etapaDetalle = expediente.EtapaDetalle?.Nombre,
                        asesor = expediente.Usuario != null ? $"{expediente.Usuario.Username}" : "",
                        miniatura = ThumbnailHelper.TryGetThumbnailBase64(expediente.Ubicacion, expediente.NombreArchivoHash)
                    };
                }
                else
                {
                    response.data = null;
                }
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

        [HttpGet("document/{id}")]
        public async Task<IActionResult> GetDocument([FromHeader] string Authorization, int id)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Expediente";
            try
            {
                Expediente? expediente = await casesService.GetByIdAsync(id);
                if (expediente == null)
                {
                    response.code = "404";
                    response.message = "Expediente no encontrado";
                    return NotFound(response);
                }
                response.data = casesLogic.GetMainDocument(expediente);
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
