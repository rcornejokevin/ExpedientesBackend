
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
        public async Task newCases(Expediente expediente, string archivoBase64)
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
            expedienteDetalle.EtapaAnteriorId = expediente?.EtapaId ?? 0;
            expedienteDetalle.EtapaDetalleAnteriorId = expediente?.EtapaDetalleId == 0 ? null : expediente?.EtapaDetalleId;
            expedienteDetalle.AsesorAnteriorId = expediente?.AsesorId ?? 0;
            if (followCase != null)
            {
                expedienteDetalle.EtapaNuevaId = followCase.etapaId;
                expedienteDetalle.EtapaDetalleNuevaId = followCase.subEtapaId == 0 ? null : followCase.subEtapaId;
                //expedienteDetalle.AsesorNuevorId = followCase.asesor;
            }

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
                expediente.NombreArchivo = followCaseVar.nombreArchivo;
                var hash = HashNombre(followCaseVar.nombreArchivo);
                var extension = Path.GetExtension(followCaseVar.nombreArchivo);
                expediente.NombreArchivoHash = string.IsNullOrWhiteSpace(extension)
                    ? hash
                    : $"{hash}{extension}";

                if (!followCaseVar.adjuntarArchivo) await fileLogic.SaveBase64Async(followCaseVar.archivo, uploadDir, expediente.NombreArchivoHash);

                expediente.Ubicacion = uploadDir;
            }
            //expediente.AsesorId = followCaseVar.asesor;
            expediente.EtapaId = followCaseVar.etapaId;
            expediente.EtapaDetalleId = followCaseVar.subEtapaId;
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
                if (subEtapaNew.Orden < subEtapaOld.Orden)
                {
                    throw new ArgumentException("La nueva subetapa debe ser mayor en orden que la anterior.");
                }
            }
            return true;

        }
    }
}
