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
        public FlujoService flujoService;
        public UsuarioService usuarioService;
        public CasesNoteService casesNoteService;
        [ActivatorUtilitiesConstructor]
        public CasesController(Jwt jwt, CasesLogic casesLogic, CasesService casesService, CasesDetailService casesDetailService, UsuarioService usuarioService, FlujoService flujoService,
        CasesNoteService casesNoteService)
        {
            this.jwt = jwt;
            this.casesLogic = casesLogic;
            this.casesService = casesService;
            this.casesDetailService = casesDetailService;
            this.usuarioService = usuarioService;
            this.flujoService = flujoService;
            this.casesNoteService = casesNoteService;
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
            Flujo? flujo = await flujoService.GetByIdAsync(casesRequest.flujoId);
            if (flujo == null)
            {
                response.code = "400";
                response.message = "Flujo no existente";
                response.data = new { };
                return Ok(response);
            }

            response.code = "000";
            response.message = "Creacion de expediente correcta";
            Expediente expediente = new Expediente();
            expediente.ExpedienteRelacionadoId = null;
            if (flujo.FlujoAsociado == 1)
            {
                if (casesRequest.expedienteRelacionadoId != 0)
                {
                    Expediente? expedienteRelacionado = await casesService.GetByIdAsync(casesRequest.expedienteRelacionadoId);
                    if (expedienteRelacionado == null)
                    {
                        response.code = "400";
                        response.message = "Expediente Relacionado no encontrado";
                        response.data = new { };
                        return Ok(response);
                    }
                    expediente.ExpedienteRelacionadoId = casesRequest.expedienteRelacionadoId;
                }

            }
            expediente.Codigo = await casesLogic.GetCodeByFlujo(flujo);
            expediente.Nombre = casesRequest.nombre;
            expediente.Ubicacion = "";
            expediente.NombreArchivo = casesRequest.nombreArchivo;
            expediente.FechaIngreso = casesRequest.fechaIngreso;
            expediente.FechaActualizacion = DateTime.UtcNow;
            expediente.AsesorId = casesRequest.asesor;
            expediente.Estatus = "Abierto";
            expediente.Activo = 1;
            expediente.CampoValorJson = casesRequest.campos;
            expediente.RemitenteId = casesRequest.remitenteId;
            expediente.Asunto = casesRequest.asunto;
            expediente.EtapaDetalleId = null;
            try
            {
                await casesLogic.newCases(expediente, casesRequest.archivo, casesRequest.flujoId);

                response.data = new
                {
                    expediente.Codigo
                };
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
            response.code = "000";
            response.message = "Edicion de expediente correcta";
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
                subEtapaId = casesRequest.subEtapaId ?? null,
                adjuntarArchivo = casesRequest.adjuntarArchivo,
                nombreArchivo = casesRequest.nombreArchivo,
                archivo = casesRequest.archivo,
                asesor = casesRequest.asesor,
                campos = casesRequest.campos
            };
            if (casesLogic.userCanChangeCase(user))
            {
                expedienteOld.Asunto = string.IsNullOrEmpty(casesRequest.asunto) ? expedienteOld.Asunto : casesRequest.asunto;
                expedienteOld.Nombre = string.IsNullOrEmpty(casesRequest.nombre) ? expedienteOld.Nombre : casesRequest.nombre;
                expedienteOld.FechaIngreso = casesRequest.fechaIngreso;
                expedienteOld.RemitenteId = casesRequest.remitenteId;
                expedienteOld.ExpedienteRelacionadoId = casesRequest.expedienteRelacionadoId == 0 ? null : casesRequest.expedienteRelacionadoId;
            }
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
                await casesLogic.newDetailForChangeStatus(expediente, casesRequest.state);
                expediente.Estatus = casesRequest.state;
                await casesService.EditAsync(expediente);
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
         int? reporteId = 0,
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
                if (user.Perfil == "ASESOR")
                {
                    filters.UsuarioAsesor = user.Id;
                }
                else if (user.Perfil != "ADMINISTRADOR" && user.Perfil != "RECEPCIÓN" && (user.Perfil != "PROCURADOR" || reporteId != 1))
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
                    Activo = e.Activo == 1 ? true : false,
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
                        Activo = expediente.Activo == 1 ? true : false,
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
                        puedeArchivarse = expediente.Etapa.Flujo.CierreArchivado,
                        puedeDevolverseAlRemitente = expediente.Etapa.Flujo.CierreDevolucionAlRemitente,
                        puedeEnviarAJudicial = expediente.Etapa.Flujo.CierreEnviadoAJudicial,
                        expediente.Asunto,
                        flujoId = expediente.Etapa != null ? expediente.Etapa.FlujoId : 0,
                        flujo = expediente.Etapa?.Flujo.Nombre,
                        etapa = expediente.Etapa?.Nombre,
                        etapaDetalle = expediente.EtapaDetalle?.Nombre,
                        expedienteRelacionado = (expediente.ExpedienteRelacionadoId != null && expediente.ExpedienteRelacionadoId != 0) ? expediente.ExpedienteRelacionado.Codigo : "",
                        cantidadDocumentos = await casesDetailService.CountByExpedienteIdAsync(expediente.Id),
                        asesor = expediente.Usuario != null ? $"{expediente.Usuario.Username}" : "",
                        miniatura = ThumbnailHelper.TryGetThumbnailBase64(expediente.Ubicacion, expediente.NombreArchivoHash),
                        puedeRelacionarse = expediente.Etapa?.Flujo.FlujoAsociado,
                        expedienteRelacionadoId = expediente.ExpedienteRelacionadoId,
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
        [HttpGet("document_detail/{id}")]
        public async Task<IActionResult> GetDocumentDetail([FromHeader] string Authorization, int id)
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
                ExpedienteDetalle? expedienteDetalle = await casesDetailService.GetByIdAsync(id);
                if (expedienteDetalle == null)
                {
                    response.code = "404";
                    response.message = "Expediente no encontrado";
                    return Ok(response);
                }
                response.data = casesLogic.GetDocumentDetail(expedienteDetalle);
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
                        asesorNuevo = d.AsesorNuevo.Username,
                        etapa = d.EtapaNueva.Nombre,
                        subEtapa = d.EtapaDetalleNuevaId != null ? d.EtapaDetalleNueva?.Nombre : "",
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
        [HttpPost("note")]
        public async Task<IActionResult> addNoteElement([FromHeader] string Authorization, [FromBody] NewNoteRequest casesRequest)
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
            Expediente? expediente = await casesService.GetByIdAsync(casesRequest.expedienteId);
            if (expediente == null)
            {
                response.code = "404";
                response.message = "Expediente no encontrado";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Nueva Nota agregada";
            try
            {
                await casesLogic.addNote(expediente, casesRequest.nota, user);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                return Ok(response);
            }
            return Ok(response);

        }
        [HttpGet("notes/{id}")]
        public async Task<IActionResult> GetNotes([FromHeader] string Authorization, int id)
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
                List<ExpedienteNotas> notas = await casesNoteService.GetAllByExpedienteAsync(expediente);
                response.data = notas
                    .OrderBy(d => d.FechaIngreso)
                    .Select(d => new
                    {
                        d.Id,
                        asesor = d.Asesor.Username,
                        d.FechaIngreso,
                        d.Nota
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
    }
}
