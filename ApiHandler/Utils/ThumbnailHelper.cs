using System;
using System.IO;

namespace ApiHandler.Utils
{
    public static class ThumbnailHelper
    {
        // Busca una miniatura junto al PDF (mismo nombre hash) y devuelve base64.
        // Convenciones buscadas: <hash>_thumb.jpg|png o <hash>.jpg|png
        public static string? TryGetThumbnailBase64(string? ubicacion, string? nombreArchivoHash)
        {
            if (string.IsNullOrWhiteSpace(ubicacion) || string.IsNullOrWhiteSpace(nombreArchivoHash))
                return null;

            try
            {
                var baseName = Path.GetFileNameWithoutExtension(nombreArchivoHash);
                var candidates = new[]
                {
                    Path.Combine(ubicacion, $"{baseName}_thumb.jpg"),
                    Path.Combine(ubicacion, $"{baseName}_thumb.png"),
                    Path.Combine(ubicacion, $"{baseName}.jpg"),
                    Path.Combine(ubicacion, $"{baseName}.png"),
                };

                foreach (var path in candidates)
                {
                    if (File.Exists(path))
                    {
                        var bytes = File.ReadAllBytes(path);
                        return Convert.ToBase64String(bytes);
                    }
                }
            }
            catch
            {
                // Ignorar errores y devolver null
            }
            return null;
        }
    }
}

