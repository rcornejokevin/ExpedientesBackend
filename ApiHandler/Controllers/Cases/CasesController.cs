using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using ApiHandler.Models.Cases;
using BusinessLogic.Services;
using DBHandler.Models;
using DBHandler.Service.Catalog;
using ApiHandler.Utils;
using BusinessLogic.Models;
using DBHandler.Service.Cases;
using DBHandler.Service.Security;
namespace ApiHandler.Controllers.Catalog
{
    [Route("cases")]
    [ApiController]
    public class CasesController : Controller
    {
        public Jwt jwt;
        public CasesLogic casesLogic;
        public CasesService casesService;
        public CasesDetailService casesDetailService;
        public UsuarioService usuarioService;
        [ActivatorUtilitiesConstructor]
        public CasesController(Jwt jwt, CasesLogic casesLogic, CasesService casesService, CasesDetailService casesDetailService, UsuarioService usuarioService)
        {
            this.jwt = jwt;
            this.casesLogic = casesLogic;
            this.casesService = casesService;
            this.casesDetailService = casesDetailService;
            this.usuarioService = usuarioService;
        }
        [HttpGet("indicators")]
        public async Task<IActionResult> GetIndicators([FromHeader] string Authorization)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }

            response.code = "000";
            response.message = "Indicadores del mes actual";
            try
            {
                string? username = jwt.GetUsernameFromAuthorization(Authorization);
                if (string.IsNullOrWhiteSpace(username))
                {
                    response.code = "401";
                    response.message = "Unauthorized";
                    return Ok(response);
                }
                Usuario user = await usuarioService.getUsuarioByUsername(username) ?? new Usuario();
                if (user.Id == 0)
                {
                    response.code = "401";
                    response.message = "Unauthorized";
                    return Ok(response);
                }
                var data = await casesLogic.GetMonthlyIndicatorsForUser(user);
                response.data = data;
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
        public async Task<IActionResult> addElement([FromHeader] string Authorization, [FromBody] NewCasesRequest casesRequest)
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
            response.message = "Detalle de campo";
            Expediente expediente = new Expediente();
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
            expediente.RemitenteId = casesRequest.remitenteId;
            expediente.Asunto = casesRequest.asunto;
            try
            {
                await casesLogic.newCases(expediente, casesRequest.archivo, casesRequest.flujoId);

            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                return Ok(response);
            }
            return Ok(response);

        }
        [HttpPut("edit")]
        public async Task<IActionResult> editElement([FromHeader] string Authorization, [FromBody] EditCasesRequest casesRequest)
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
            response.message = "Detalle de campo";
            Expediente? expedienteOld = await casesService.GetByIdAsync(casesRequest.id);
            if (expedienteOld == null)
            {
                response.code = "404";
                response.message = "Expediente no encontrado";
                return Ok(response);
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
            response.message = "Expediente editado correctamente";
            response.data = expedienteOld;
            return Ok(response);
        }
        [HttpPut("editState")]
        public async Task<IActionResult> editState([FromHeader] string Authorization, [FromBody] EditCaseStateRequest casesRequest)
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
            response.message = "Detalle de campo";
            Expediente? expediente = await casesService.GetByIdAsync(casesRequest.id);
            if (expediente == null)
            {
                response.code = "404";
                response.message = "Expediente no encontrado";
                return Ok(response);
            }
            try
            {
                string? username = jwt.GetUsernameFromAuthorization(Authorization);
                if (string.IsNullOrWhiteSpace(username))
                {
                    response.code = "401";
                    response.message = "Unauthorized";
                    return Ok(response);
                }
                Usuario user = await usuarioService.getUsuarioByUsername(username) ?? new Usuario();
                if (user.Id == 0)
                {
                    response.code = "401";
                    response.message = "Unauthorized";
                    return Ok(response);
                }
                if (!casesLogic.userCanChangeState(casesRequest.state, user))
                {
                    response.code = "401";
                    response.message = "Usuario no puede cerrar este archivo";
                    return Ok(response);
                }
                if (!casesLogic.caseCanChangeState(expediente))
                {
                    response.code = "401";
                    response.message = "Expediente no puede cambiar de estado";
                    return Ok(response);
                }
                expediente.Estatus = casesRequest.state;
                await casesService.EditAsync(expediente);
                await casesLogic.newDetailForChangeStatus(expediente, casesRequest.state);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.ToString();
                return Ok(response);
            }
            response.code = "000";
            response.message = "Expediente editado correctamente";
            response.data = expediente;
            return Ok(response);
        }
        [HttpGet("list")]
        public async Task<IActionResult> List([FromHeader] string Authorization,
         int limit = 100,
         string? fechaInicioIngreso = "",
         string? fechaFinIngreso = "",
         string? fechaInicioActualizacion = "",
         string? fechaFinActualizacion = "",
         int? asesorId = 0,
         int? flujoId = 0,
         int? etapaId = 0,
         int? subEtapaId = 0,
         string? estatus = "",
         string? asunto = "",
         int? remitenteId = 0
         )
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            string? username = jwt.GetUsernameFromAuthorization(Authorization);
            if (username == null)
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            Usuario user = await usuarioService.getUsuarioByUsername(username) ?? new Usuario();
            if (user.Id == 0)
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Listado de Expedientes";
            try
            {
                Filters filters = new Filters
                {
                    FechaInicioIngreso = fechaInicioIngreso,
                    FechaFinIngreso = fechaFinIngreso,
                    FechaInicioActualizacion = fechaInicioActualizacion,
                    FechaFinActualizacion = fechaFinActualizacion,
                    AsesorId = asesorId,
                    FlujoId = flujoId,
                    EtapaId = etapaId,
                    SubEtapaId = subEtapaId,
                    Estatus = estatus,
                    Asunto = asunto,
                    RemitenteId = remitenteId,
                    Limit = limit
                };
                if (user.Perfil != "Administrador")
                {
                    filters.Usuario = user.Id;
                }
                var expedientes = await casesService.GetAllAsync(filters);
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
                    e.RemitenteId,
                    Remitente = e.Remitente.Descripcion,
                    e.Asunto,
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
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
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
                        expediente.RemitenteId,
                        Remitente = expediente.Remitente.Descripcion,
                        puedeCerrarse = expediente.Etapa.FinDeFlujo,
                        expediente.Asunto,
                        flujoId = expediente.Etapa != null ? expediente.Etapa.FlujoId : 0,
                        flujo = expediente.Etapa?.Flujo.Nombre,
                        etapa = expediente.Etapa?.Nombre,
                        etapaDetalle = expediente.EtapaDetalle?.Nombre,
                        cantidadDocumentos = await casesDetailService.CountByExpedienteIdAsync(expediente.Id),
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
                return Ok(response);
            }
            return Ok(response);
        }

        [HttpGet("document/{id}")]
        public async Task<IActionResult> GetDocument([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Expediente";
            try
            {
                Expediente? expediente = await casesService.GetByIdAsync(id);
                if (expediente == null)
                {
                    response.code = "404";
                    response.message = "Expediente no encontrado";
                    return Ok(response);
                }
                response.data = casesLogic.GetMainDocument(expediente);
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
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> getDetail([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Lista de Expediente Detalle";
            try
            {
                List<ExpedienteDetalle> expedienteDetalle = await casesDetailService.GetAllByExpedienteIdAsync(id);
                response.data = expedienteDetalle
                    .OrderBy(d => d.Fecha)
                    .Select(d => new
                    {
                        d.Id,
                        d.Fecha,
                        d.Ubicacion,
                        d.NombreArchivo,
                        d.NombreArchivoHash,
                        d.EtapaAnteriorId,
                        d.EtapaDetalleAnteriorId,
                        d.EtapaNuevaId,
                        d.EtapaDetalleNuevaId,
                        d.AsesorAnteriorId,
                        d.AsesorNuevorId,
                        etapa = d.EtapaNueva.Nombre,
                        subEtapa = d.EtapaDetalleNuevaId != null ? d.EtapaDetalleNueva?.Nombre : "",
                        miniatura = ThumbnailHelper.TryGetThumbnailBase64(d.Ubicacion ?? string.Empty, d.NombreArchivoHash ?? string.Empty),
                        Estatus = d.EstatusNuevo,
                    })
                    .ToList();
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
        [HttpGet("detail_file/{id}")]
        public async Task<IActionResult> getDetailFile([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Expediente Detalle";
            try
            {
                ExpedienteDetalle? expedienteDetalle = await casesDetailService.GetByIdAsync(id);
                response.data = expedienteDetalle;
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
