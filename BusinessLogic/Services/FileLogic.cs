using System.IO;

namespace BusinessLogic.Services
{
    public class FileLogic
    {
        public async Task UploadFileAsync(FileInfo file, string uploadPath)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (string.IsNullOrWhiteSpace(uploadPath)) throw new ArgumentException("Ruta de carga inválida", nameof(uploadPath));

            if (!file.Exists)
            {
                throw new FileNotFoundException($"Archivo origen no encontrado: {file.FullName}");
            }

            Directory.CreateDirectory(uploadPath);

            var extension = Path.GetExtension(file.Name);
            var baseName = Path.GetFileNameWithoutExtension(file.Name);
            var destinationPath = Path.Combine(uploadPath, file.Name);

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

            using (var source = file.OpenRead())
            using (var target = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await source.CopyToAsync(target);
            }
        }
        public string ConvertFileToBase64(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }
        public async Task SaveBase64Async(string base64Content, string uploadPath, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(base64Content)) throw new ArgumentException("Contenido base64 vacío", nameof(base64Content));
            if (string.IsNullOrWhiteSpace(uploadPath)) throw new ArgumentException("Ruta de carga inválida", nameof(uploadPath));
            if (string.IsNullOrWhiteSpace(destFileName)) throw new ArgumentException("Nombre de archivo destino inválido", nameof(destFileName));

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

            try
            {
                var ext = (Path.GetExtension(destFileName) ?? string.Empty).ToLowerInvariant();
                var baseName = Path.GetFileNameWithoutExtension(destFileName);
                if (string.IsNullOrWhiteSpace(baseName)) baseName = destFileName;

                if (ext == ".pdf")
                {
                    if (!TryGeneratePdfThumbWithPdftoppm(destinationPath, uploadPath, baseName))
                    {
                        TryGeneratePdfThumbWithGhostscript(destinationPath, uploadPath, baseName);
                    }
                }
                else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    var thumbExt = ext;
                    var thumbPath = Path.Combine(uploadPath, $"{baseName}_thumb{thumbExt}");
                    if (!File.Exists(thumbPath))
                    {
                        File.Copy(destinationPath, thumbPath, overwrite: false);
                    }
                }
            }
            catch
            {
                // Silencio: la miniatura es opcional. No romper el flujo de guardado.
            }
        }

        private static bool TryGeneratePdfThumbWithPdftoppm(string pdfPath, string outputDir, string baseName)
        {
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "pdftoppm";
                process.StartInfo.ArgumentList.Add("-f");
                process.StartInfo.ArgumentList.Add("1");
                process.StartInfo.ArgumentList.Add("-l");
                process.StartInfo.ArgumentList.Add("1");
                process.StartInfo.ArgumentList.Add("-png");
                process.StartInfo.ArgumentList.Add("-scale-to");
                process.StartInfo.ArgumentList.Add("600");
                process.StartInfo.ArgumentList.Add(pdfPath);
                var tempBase = System.IO.Path.Combine(outputDir, baseName);
                process.StartInfo.ArgumentList.Add(tempBase);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                if (!process.WaitForExit(10000))
                {
                    try { process.Kill(entireProcessTree: true); } catch { }
                    return false;
                }
                var generated = System.IO.Path.Combine(outputDir, $"{baseName}-1.png");
                var thumb = System.IO.Path.Combine(outputDir, $"{baseName}_thumb.png");
                if (File.Exists(generated))
                {
                    if (File.Exists(thumb)) File.Delete(thumb);
                    File.Move(generated, thumb);
                    return true;
                }
            }
            catch { }
            return false;
        }

        private static bool TryGeneratePdfThumbWithGhostscript(string pdfPath, string outputDir, string baseName)
        {
            try
            {
                var thumb = System.IO.Path.Combine(outputDir, $"{baseName}_thumb.png");
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "gs";
                process.StartInfo.ArgumentList.Add("-sDEVICE=png16m");
                process.StartInfo.ArgumentList.Add("-dFirstPage=1");
                process.StartInfo.ArgumentList.Add("-dLastPage=1");
                process.StartInfo.ArgumentList.Add("-r150");
                process.StartInfo.ArgumentList.Add("-o");
                process.StartInfo.ArgumentList.Add(thumb);
                process.StartInfo.ArgumentList.Add(pdfPath);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                if (!process.WaitForExit(10000))
                {
                    try { process.Kill(entireProcessTree: true); } catch { }
                    return false;
                }
                return File.Exists(thumb);
            }
            catch { }
            return false;
        }
    }
}
