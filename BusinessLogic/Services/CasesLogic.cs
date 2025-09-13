
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

        public CasesLogic(CasesService casesService, FileLogic fileLogic, IConfiguration configuration, CasesDetailService casesDetailService, EtapaService etapaService, EtapaDetalleService etapaDetalleService)
        {
            this.casesService = casesService;
            this.fileLogic = fileLogic;
            this.configuration = configuration;
            this.casesDetailService = casesDetailService;
            this.etapaService = etapaService;
            this.etapaDetalleService = etapaDetalleService;
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
        public bool userCanChangeState(string state, Usuario usuario)
        {
            if (usuario.Perfil != "Administrador" && state == "Archivado") return false;
            return true;
        }
        public bool caseCanChangeState(Expediente expediente)
        {
            if (expediente.Etapa.FinDeFlujo)
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
            if (expediente.EtapaDetalleId == 0 && expediente.EtapaId == 0)
            {
                throw new ArgumentException("El expediente debe tener una etapa o subetapa asignada.");
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

                    // Genera un nuevo nombre de archivo (hash + extensión) y conserva el original en carpeta
                    var extension = Path.GetExtension(expediente.NombreArchivo);
                    var newHash = HashNombre(expediente.NombreArchivo);
                    var newFileName = string.IsNullOrWhiteSpace(extension) ? newHash : $"{newHash}{extension}";
                    var outPath = Path.Combine(currentDir, newFileName);

                    await fileLogic.AppendPdfBase64ToNewFileAsync(followCaseVar.archivo, targetPath, outPath);

                    // Actualiza el expediente para apuntar al nuevo archivo combinado
                    expediente.NombreArchivoHash = newFileName;
                    expediente.Ubicacion = currentDir;
                }
            }
            //expediente.AsesorId = followCaseVar.asesor;
            expediente.EtapaId = followCaseVar.etapaId;
            expediente.EtapaDetalleId = followCaseVar.subEtapaId;
            expediente.AsesorId = followCaseVar.asesor == 0 ? expediente.AsesorId : followCaseVar.asesor;
            await casesService.EditAsync(expediente);
            await newDetailCasesByCases(expediente, followCaseVar);
        }
        public async Task<Boolean> checkCases(Expediente? oldExpediente, FollowCase followCase)
        {
            if (oldExpediente == null)
            {
                throw new ArgumentException("El expediente no existe.");
            }
            if (oldExpediente.EtapaId == followCase.etapaId && oldExpediente.EtapaDetalleId == followCase.subEtapaId)
            {
                throw new ArgumentException("No se detecto un cambio en la etapa o subetapa.");
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
            if (etapaNew.Orden < etapaOld.Orden)
            {
                throw new ArgumentException("La nueva etapa debe ser mayor en orden que la anterior.");
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
            if (followCase.subEtapaId != 0)
            {
                subEtapaNew = await etapaDetalleService.GetByIdAsync(followCase.subEtapaId);
                if (subEtapaNew == null)
                {
                    throw new ArgumentException("La subetapa no existe.");
                }
            }
            if (subEtapaNew != null && subEtapaOld != null)
            {
                if (etapaOld.Id == etapaNew.Id && subEtapaNew.Orden < subEtapaOld.Orden)
                {
                    throw new ArgumentException("La nueva subetapa debe ser mayor en orden que la anterior.");
                }
            }
            return true;

        }

        public async Task<object> GetMonthlyIndicatorsForUser(Usuario user)
        {
            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Local);
            var nextMonthStart = monthStart.AddMonths(1);

            var details = await casesDetailService.GetAllByDateRangeWithEtapaAsync(monthStart, nextMonthStart);

            var assigned = details.Where(d => d.AsesorNuevorId == user.Id).ToList();
            var attended = details.Where(d => d.AsesorAnteriorId == user.Id).ToList();

            // Serie por día del mes actual
            var daysInMonth = Enumerable.Range(0, (nextMonthStart - monthStart).Days)
                .Select(offset => monthStart.AddDays(offset).Date)
                .ToList();

            var series = daysInMonth.Select(day => new
            {
                date = day.ToString("yyyy-MM-dd"),
                assigned = assigned.Count(d => d.Fecha.Date == day),
                attended = attended.Count(d => d.Fecha.Date == day)
            }).ToList();

            // Tipos por flujo (asignados)
            var assignedByType = assigned
                .GroupBy(d => new
                {
                    flujoId = d.EtapaNueva?.FlujoId ?? 0,
                    flujo = d.EtapaNueva?.Flujo?.Nombre ?? string.Empty
                })
                .Select(g => new
                {
                    flowId = g.Key.flujoId,
                    flow = string.IsNullOrWhiteSpace(g.Key.flujo) ? $"Flujo {g.Key.flujoId}" : g.Key.flujo,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToList();

            // Tipos por flujo (atendidos)
            var attendedByType = attended
                .GroupBy(d => new
                {
                    flujoId = d.EtapaNueva?.FlujoId ?? 0,
                    flujo = d.EtapaNueva?.Flujo?.Nombre ?? string.Empty
                })
                .Select(g => new
                {
                    flowId = g.Key.flujoId,
                    flow = string.IsNullOrWhiteSpace(g.Key.flujo) ? $"Flujo {g.Key.flujoId}" : g.Key.flujo,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToList();

            var totalAssigned = assigned.Count;
            var totalAttended = attended.Count;

            var assignedByTypeWithPct = assignedByType
                .Select(x => new
                {
                    x.flowId,
                    x.flow,
                    x.count,
                    percent = totalAssigned == 0 ? 0 : Math.Round((decimal)x.count * 100m / totalAssigned, 2)
                })
                .ToList();

            var attendedByTypeWithPct = attendedByType
                .Select(x => new
                {
                    x.flowId,
                    x.flow,
                    x.count,
                    percent = totalAttended == 0 ? 0 : Math.Round((decimal)x.count * 100m / totalAttended, 2)
                })
                .ToList();

            return new
            {
                month = monthStart.ToString("yyyy-MM"),
                totals = new { assigned = totalAssigned, attended = totalAttended },
                series,
                assignedByType = assignedByTypeWithPct,
                attendedByType = attendedByTypeWithPct
            };
        }
    }
}
