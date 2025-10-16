
using BusinessLogic.Models;
using DBHandler.Models;
using DBHandler.Service.Cases;
using DBHandler.Service.Catalog;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services
{
    public class CasesLogic
    {
        private readonly IConfiguration configuration;
        public CasesService casesService;
        public FileLogic fileLogic;
        public CasesDetailService casesDetailService;
        public EtapaService etapaService;
        public EtapaDetalleService etapaDetalleService;
        public CasesNoteService casesNoteService;

        public CasesLogic(CasesService casesService, FileLogic fileLogic, IConfiguration configuration, CasesDetailService casesDetailService, EtapaService etapaService, EtapaDetalleService etapaDetalleService, CasesNoteService casesNoteService)
        {
            this.casesService = casesService;
            this.fileLogic = fileLogic;
            this.configuration = configuration;
            this.casesDetailService = casesDetailService;
            this.etapaService = etapaService;
            this.etapaDetalleService = etapaDetalleService;
            this.casesNoteService = casesNoteService;
        }
        public async Task<string> GetCodeByFlujo(Flujo flujo)
        {
            Filters filters = new Filters();
            List<Expediente> list = await casesService.GetAllAsync(filters);
            int correlativo = list.Count() + 1;
            string correlativoString = correlativo.ToString().PadLeft(3, '0');
            return correlativoString + flujo.Correlativo + DateTime.UtcNow.Year;
        }
        public static string HashNombre(string nombre)
        {
            string randomPart = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string input = $"{nombre}{randomPart}";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }
        public object GetMainDocument(Expediente expediente)
        {

            return new
            {
                expediente.NombreArchivo,
                Archivo = fileLogic.ConvertFileToBase64(expediente.Ubicacion + expediente.NombreArchivoHash)
            };
        }
        public object GetDocumentDetail(ExpedienteDetalle expedienteDetalle)
        {

            return new
            {
                expedienteDetalle.NombreArchivo,
                Archivo = fileLogic.ConvertFileToBase64(expedienteDetalle.Ubicacion + expedienteDetalle.NombreArchivoHash)
            };
        }
        public bool userCanChangeState(string state, Usuario usuario)
        {
            if (usuario.Perfil != "ADMINISTRADOR" && state == "Archivado") return false;
            return true;
        }
        public bool caseCanChangeState(Expediente expediente)
        {
            if (expediente.Etapa.FinDeFlujo == 1)
            {
                return true;
            }
            return false;
        }
        public async Task newCases(Expediente expediente, string archivoBase64, int flujoId)
        {
            string? uploadDir = configuration["FtpStrings:Location"] ?? null;
            if (uploadDir == null || !Directory.Exists(uploadDir))
            {
                throw new DirectoryNotFoundException("El directorio de carga no existe. ");
            }
            if (!string.IsNullOrWhiteSpace(expediente.NombreArchivo) && !string.IsNullOrWhiteSpace(archivoBase64))
            {
                var hash = HashNombre(expediente.NombreArchivo);
                var extension = Path.GetExtension(expediente.NombreArchivo);
                expediente.NombreArchivoHash = string.IsNullOrWhiteSpace(extension)
                    ? hash
                    : $"{hash}{extension}";

                await fileLogic.SaveBase64Async(archivoBase64, uploadDir, expediente.NombreArchivoHash);

                expediente.Ubicacion = uploadDir;
            }

            Etapa? etapaFirst = await etapaService.getFirstEtapaByFlujoId(flujoId);
            if (etapaFirst != null)
            {
                expediente.EtapaId = etapaFirst.Id;
                EtapaDetalle? subEtapaFirst = await etapaDetalleService.getFirstEtapaDetalleByEtapaId(etapaFirst.Id);
                if (subEtapaFirst != null)
                {
                    expediente.EtapaDetalleId = subEtapaFirst.Id;
                }
            }
            await casesService.AddAsync(expediente);
            await this.newDetailCasesByCases(expediente, null);
        }
        public async Task<ExpedienteDetalle> newDetailCasesByCases(Expediente expediente, FollowCase? followCase)
        {
            if (expediente == null)
            {
                throw new ArgumentException("El expediente no existe.");
            }
            ExpedienteDetalle expedienteDetalle = new ExpedienteDetalle();
            expedienteDetalle.ExpedienteId = expediente.Id;
            expedienteDetalle.Fecha = DateTime.UtcNow;
            expedienteDetalle.Ubicacion = expediente.Ubicacion;
            expedienteDetalle.NombreArchivo = expediente.NombreArchivo;
            expedienteDetalle.NombreArchivoHash = expediente.NombreArchivoHash;
            expedienteDetalle.EstatusAnterior = expediente.Estatus;
            expedienteDetalle.EstatusNuevo = expediente.Estatus;
            if (followCase != null)
            {
                expedienteDetalle.EtapaAnteriorId = expediente.EtapaId;
                expedienteDetalle.EtapaDetalleAnteriorId = expediente?.EtapaDetalleId == 0 ? null : expediente?.EtapaDetalleId;
                expedienteDetalle.AsesorAnteriorId = expediente.AsesorId;
                expedienteDetalle.EtapaNuevaId = followCase.etapaId;
                expedienteDetalle.EtapaDetalleNuevaId = followCase.subEtapaId == 0 ? null : followCase.subEtapaId;
                expedienteDetalle.AsesorNuevorId = followCase.asesor == 0 ? expediente.AsesorId : followCase.asesor;
            }
            else
            {
                expedienteDetalle.EtapaNuevaId = expediente.EtapaId;
                expedienteDetalle.EtapaDetalleNuevaId = expediente.EtapaDetalleId == 0 ? null : expediente.EtapaDetalleId;
                expedienteDetalle.AsesorNuevorId = expediente.AsesorId;
            }

            return await casesDetailService.AddAsync(expedienteDetalle);
        }
        public async Task<ExpedienteDetalle> newDetailForChangeStatus(Expediente expediente, string status)
        {
            if (expediente == null)
            {
                throw new ArgumentException("El expediente no existe.");
            }
            ExpedienteDetalle expedienteDetalle = new ExpedienteDetalle();
            expedienteDetalle.ExpedienteId = expediente.Id;
            expedienteDetalle.Fecha = DateTime.UtcNow;
            expedienteDetalle.Ubicacion = expediente.Ubicacion;
            expedienteDetalle.NombreArchivo = expediente.NombreArchivo;
            expedienteDetalle.NombreArchivoHash = expediente.NombreArchivoHash;
            expedienteDetalle.EstatusAnterior = expediente.Estatus;
            expedienteDetalle.EstatusNuevo = status;
            expedienteDetalle.EtapaAnteriorId = expediente.EtapaId;
            expedienteDetalle.EtapaNuevaId = expediente.EtapaId;
            expedienteDetalle.EtapaDetalleAnteriorId = expediente.EtapaDetalleId;
            expedienteDetalle.EtapaDetalleNuevaId = expediente.EtapaDetalleId;
            expedienteDetalle.AsesorAnteriorId = expediente.AsesorId;
            expedienteDetalle.AsesorNuevorId = expediente.AsesorId;
            return await casesDetailService.AddAsync(expedienteDetalle);
        }
        public async Task followCase(Expediente expediente, FollowCase followCaseVar)
        {
            string? uploadDir = configuration["FtpStrings:Location"] ?? null;
            if (uploadDir == null || !Directory.Exists(uploadDir))
            {
                throw new DirectoryNotFoundException("El directorio de carga no existe. ");
            }

            await checkCases(expediente, followCaseVar);
            if (!string.IsNullOrWhiteSpace(followCaseVar.nombreArchivo) && !string.IsNullOrWhiteSpace(followCaseVar.archivo))
            {

                expediente.Ubicacion = uploadDir;
                if (!followCaseVar.adjuntarArchivo)
                {
                    expediente.NombreArchivo = followCaseVar.nombreArchivo;
                    var hash = HashNombre(followCaseVar.nombreArchivo);
                    var extension = Path.GetExtension(followCaseVar.nombreArchivo);
                    expediente.NombreArchivoHash = string.IsNullOrWhiteSpace(extension)
                        ? hash
                        : $"{hash}{extension}";
                    await fileLogic.SaveBase64Async(followCaseVar.archivo, uploadDir, expediente.NombreArchivoHash);

                }
                else
                {
                    var currentDir = string.IsNullOrWhiteSpace(expediente.Ubicacion) ? uploadDir : expediente.Ubicacion;
                    var currentName = expediente.NombreArchivoHash;
                    if (string.IsNullOrWhiteSpace(currentDir) || string.IsNullOrWhiteSpace(currentName))
                    {
                        throw new ArgumentException("No existe documento base para adjuntar.");
                    }
                    var targetPath = Path.Combine(currentDir, currentName);

                    var extension = Path.GetExtension(expediente.NombreArchivo);
                    var newHash = HashNombre(expediente.NombreArchivo);
                    var newFileName = string.IsNullOrWhiteSpace(extension) ? newHash : $"{newHash}{extension}";
                    var outPath = Path.Combine(currentDir, newFileName);

                    await fileLogic.AppendPdfBase64ToNewFileAsync(followCaseVar.archivo, targetPath, outPath);
                    expediente.NombreArchivoHash = newFileName;
                    expediente.Ubicacion = currentDir;
                }
            }
            expediente.EtapaId = followCaseVar.etapaId;
            expediente.EtapaDetalleId = followCaseVar.subEtapaId;
            expediente.AsesorId = followCaseVar.asesor == 0 ? expediente.AsesorId : followCaseVar.asesor;
            expediente.CampoValorJson = followCaseVar.campos;
            await casesService.EditAsync(expediente);
            await newDetailCasesByCases(expediente, followCaseVar);
        }
        public Boolean userCanChangeCase(Usuario user)
        {
            if (user.Perfil == "ADMINISTRADOR")
            {
                return true;
            }
            return false;
        }
        public async Task<Boolean> checkCases(Expediente? oldExpediente, FollowCase followCase)
        {
            if (oldExpediente == null)
            {
                throw new ArgumentException("El expediente no existe.");
            }
            if (followCase.etapaId == 0 && followCase.subEtapaId == 0)
            {
                throw new ArgumentException("El expediente debe tener una etapa o subetapa asignada.");
            }
            var etapaOld = await etapaService.GetEtapaByIdAsync(oldExpediente.EtapaId);
            var etapaNew = await etapaService.GetEtapaByIdAsync(followCase.etapaId);
            if (etapaOld == null || etapaNew == null)
            {
                throw new ArgumentException("La etapa no existe.");
            }
            EtapaDetalle? subEtapaOld = null;
            EtapaDetalle? subEtapaNew = null;
            if (oldExpediente.EtapaDetalleId != null)
            {
                subEtapaOld = await etapaDetalleService.GetByIdAsync(oldExpediente.EtapaDetalleId ?? 0);
                if (subEtapaOld == null)
                {
                    throw new ArgumentException("La subetapa no existe.");
                }
            }
            if (followCase.subEtapaId != 0 && followCase.subEtapaId != null)
            {
                subEtapaNew = await etapaDetalleService.GetByIdAsync(followCase.subEtapaId ?? 0);
                if (subEtapaNew == null)
                {
                    throw new ArgumentException("La subetapa no existe.");
                }
            }
            return true;

        }
        public async Task addNote(Expediente expediente, string note, Usuario usuario)
        {
            ExpedienteNotas nota = new ExpedienteNotas();
            nota.AsesorId = usuario.Id;
            nota.Nota = note;
            nota.FechaIngreso = DateTime.Now;
            nota.ExpedienteId = expediente.Id;
            await casesNoteService.AddAsync(nota);
        }
        public async Task<object> GetMonthlyIndicatorsForUser(Usuario user)
        {
            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Local);
            var nextMonthStart = monthStart.AddMonths(1);

            var details = await casesDetailService.GetAllByDateRangeWithEtapaAsync(monthStart, nextMonthStart);

            // Construir intervalos de asignación (arrastrar hasta cambio de asesor)
            var detailsByExp = details
                .OrderBy(d => d.Fecha)
                .GroupBy(d => d.ExpedienteId)
                .ToList();

            var assignmentIntervals = new List<(int expedienteId, DateTime start, DateTime end)>();
            foreach (var grp in detailsByExp)
            {
                var events = grp.OrderBy(e => e.Fecha).ToList();
                for (int i = 0; i < events.Count; i++)
                {
                    var ev = events[i];
                    var isStart = ev.AsesorNuevorId == user.Id && (ev.AsesorAnteriorId == null || ev.AsesorAnteriorId != user.Id);
                    if (!isStart) continue;

                    var start = ev.Fecha.Date;
                    var endEvent = events.Skip(i + 1).FirstOrDefault(e => e.AsesorNuevorId != user.Id);
                    var end = (endEvent != null ? endEvent.Fecha.Date : nextMonthStart.Date);

                    if (end <= monthStart.Date || start >= nextMonthStart.Date) continue;
                    if (start < monthStart.Date) start = monthStart.Date;
                    if (end > nextMonthStart.Date) end = nextMonthStart.Date;
                    if (end > start)
                    {
                        assignmentIntervals.Add((grp.Key, start, end));
                    }
                }
            }

            var daysInMonth = Enumerable.Range(0, (nextMonthStart - monthStart).Days)
                .Select(offset => monthStart.AddDays(offset).Date)
                .ToList();

            var series = daysInMonth.Select(day => new
            {
                date = day.Day.ToString(),
                assigned = assignmentIntervals
                    .Where(iv => day >= iv.start && day < iv.end)
                    .Select(iv => iv.expedienteId)
                    .Distinct()
                    .Count(),
                attended = details
                    .Where(d => d.AsesorNuevorId == user.Id)
                    .Where(d => !string.IsNullOrWhiteSpace(d.EstatusAnterior) && d.EstatusAnterior.Trim().Equals("Abierto", StringComparison.OrdinalIgnoreCase))
                    .Where(d => !string.IsNullOrWhiteSpace(d.EstatusNuevo) && d.EstatusNuevo.Trim().Equals("Cerrado", StringComparison.OrdinalIgnoreCase))
                    .Where(d => d.Fecha.Date == day)
                    .Select(d => d.ExpedienteId)
                    .Distinct()
                    .Count()
            }).ToList();

            var assignedEvents = details
                .Where(d => d.AsesorNuevorId == user.Id)
                .Where(d => !string.IsNullOrWhiteSpace(d.EstatusNuevo) && d.EstatusNuevo.Trim().Equals("Abierto", StringComparison.OrdinalIgnoreCase))
                .ToList();
            var attendedEvents = details
                .Where(d => d.AsesorNuevorId == user.Id)
                .Where(d => !string.IsNullOrWhiteSpace(d.EstatusAnterior) && d.EstatusAnterior.Trim().Equals("Abierto", StringComparison.OrdinalIgnoreCase))
                .Where(d => !string.IsNullOrWhiteSpace(d.EstatusNuevo) && d.EstatusNuevo.Trim().Equals("Cerrado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var assignedUniqueExp = assignedEvents
                .GroupBy(d => d.ExpedienteId)
                .Select(g => new
                {
                    expedienteId = g.Key,
                    flujoId = g.Select(x => x.EtapaNueva?.FlujoId ?? 0).FirstOrDefault(),
                    flujo = g.Select(x => x.EtapaNueva?.Flujo?.Nombre ?? string.Empty).FirstOrDefault()
                })
                .ToList();
            var attendedUniqueExp = attendedEvents
                .GroupBy(d => d.ExpedienteId)
                .Select(g => new
                {
                    expedienteId = g.Key,
                    flujoId = g.Select(x => x.EtapaNueva?.FlujoId ?? 0).FirstOrDefault(),
                    flujo = g.Select(x => x.EtapaNueva?.Flujo?.Nombre ?? string.Empty).FirstOrDefault()
                })
                .ToList();

            var totalAssignedExp = assignedUniqueExp.Count;
            var totalAttendedExp = attendedUniqueExp.Count;

            var assignedByType = assignedUniqueExp
                .GroupBy(x => new { x.flujoId, x.flujo })
                .Select(g => new
                {
                    flowId = g.Key.flujoId,
                    flow = string.IsNullOrWhiteSpace(g.Key.flujo) ? $"Flujo {g.Key.flujoId}" : g.Key.flujo,
                    count = g.Count(),
                    percent = totalAssignedExp == 0 ? 0 : Math.Round((decimal)g.Count() * 100m / totalAssignedExp, 2)
                })
                .OrderByDescending(x => x.count)
                .ToList();

            var attendedByType = attendedUniqueExp
                .GroupBy(x => new { x.flujoId, x.flujo })
                .Select(g => new
                {
                    flowId = g.Key.flujoId,
                    flow = string.IsNullOrWhiteSpace(g.Key.flujo) ? $"Flujo {g.Key.flujoId}" : g.Key.flujo,
                    count = g.Count(),
                    percent = totalAttendedExp == 0 ? 0 : Math.Round((decimal)g.Count() * 100m / totalAttendedExp, 2)
                })
                .OrderByDescending(x => x.count)
                .ToList();

            return new
            {
                series,
                assignedByType,
                attendedByType
            };
        }
    }
}
