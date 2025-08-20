
using DBHandler.Models;
using DBHandler.Service.Catalog;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Services
{
    public class CasesLogic
    {
        private readonly IConfiguration configuration;
        public CasesService casesService;
        public FileLogic fileLogic;

        public CasesLogic(CasesService casesService, FileLogic fileLogic, IConfiguration configuration)
        {
            this.casesService = casesService;
            this.fileLogic = fileLogic;
            this.configuration = configuration;
        }
        public async Task newCases(Expediente expediente)
        {
            string? uploadDir = configuration["FtpStrings:Location"] ?? null;
            if (uploadDir == null || !Directory.Exists(uploadDir))
            {
                throw new DirectoryNotFoundException("El directorio de carga no existe.");
            }
            if (expediente.EtapaDetalleId == 0 && expediente.EtapaId == 0)
            {
                throw new ArgumentException("El expediente debe tener una etapa o subetapa asignada.");
            }
            if (expediente.Ubicacion != null && expediente.Ubicacion.Trim() != "")
            {
                await fileLogic.UploadFileAsync(new FileInfo(expediente.Ubicacion), uploadDir);
            }
            await casesService.AddAsync(expediente);
        }
    }
}