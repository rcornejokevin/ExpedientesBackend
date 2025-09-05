
using DBHandler.Models;
using DBHandler.Service.Catalog;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
        }
    }
}
