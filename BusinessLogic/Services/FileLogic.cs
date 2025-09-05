using System.IO;

namespace BusinessLogic.Services
{
    public class FileLogic
    {
        // Copia un archivo al directorio de carga, generando un nombre único si existe.
        public async Task UploadFileAsync(FileInfo file, string uploadPath)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (string.IsNullOrWhiteSpace(uploadPath)) throw new ArgumentException("Ruta de carga inválida", nameof(uploadPath));

            // Verifica que el archivo origen exista
            if (!file.Exists)
            {
                throw new FileNotFoundException($"Archivo origen no encontrado: {file.FullName}");
            }

            // Asegura que el directorio de destino exista
            Directory.CreateDirectory(uploadPath);

            var extension = Path.GetExtension(file.Name);
            var baseName = Path.GetFileNameWithoutExtension(file.Name);
            var destinationPath = Path.Combine(uploadPath, file.Name);

            // Si ya existe, genera un nombre único manteniendo la extensión
            if (File.Exists(destinationPath))
            {
                var counter = 1;
                string candidate;
                do
                {
                    candidate = Path.Combine(uploadPath, $"{baseName}_{counter}{extension}");
                    counter++;
                } while (File.Exists(candidate));
                destinationPath = candidate;
            }

            // Copia por stream para soportar archivos grandes
            using (var source = file.OpenRead())
            using (var target = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await source.CopyToAsync(target);
            }
        }

        // Guarda contenido base64 en un archivo con nombre destino específico.
        public async Task SaveBase64Async(string base64Content, string uploadPath, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(base64Content)) throw new ArgumentException("Contenido base64 vacío", nameof(base64Content));
            if (string.IsNullOrWhiteSpace(uploadPath)) throw new ArgumentException("Ruta de carga inválida", nameof(uploadPath));
            if (string.IsNullOrWhiteSpace(destFileName)) throw new ArgumentException("Nombre de archivo destino inválido", nameof(destFileName));

            // Quita prefijo data URI si viene incluido
            var commaIndex = base64Content.IndexOf(',');
            if (base64Content.StartsWith("data:") && commaIndex > 0)
            {
                base64Content = base64Content[(commaIndex + 1)..];
            }

            Directory.CreateDirectory(uploadPath);
            var destinationPath = Path.Combine(uploadPath, destFileName);

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64Content);
            }
            catch (FormatException)
            {
                throw new ArgumentException("El contenido no es un base64 válido", nameof(base64Content));
            }

            await File.WriteAllBytesAsync(destinationPath, bytes);
        }
    }
}
