using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Services
{
    public class FileLogic
    {
        private readonly string pdftoppmCmd;
        private readonly string gsCmd;

        public FileLogic(IConfiguration configuration)
        {
            var isWindows = OperatingSystem.IsWindows();
            pdftoppmCmd = configuration["Tools:PdftoppmPath"]
                ?? (isWindows ? "pdftoppm.exe" : "pdftoppm");
            gsCmd = configuration["Tools:GhostscriptPath"]
                ?? (isWindows ? "gswin64c.exe" : "gs");
        }
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
                    if (!TryGeneratePdfThumbWithPdftoppm(destinationPath, uploadPath, baseName, pdftoppmCmd))
                    {
                        TryGeneratePdfThumbWithGhostscript(destinationPath, uploadPath, baseName, gsCmd);
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

        // Anexa un PDF (en base64) al final de un PDF existente, sobreescribiendo el archivo destino.
        public async Task AppendPdfBase64Async(string base64Content, string targetPdfPath)
        {
            if (string.IsNullOrWhiteSpace(base64Content)) throw new ArgumentException("Contenido base64 vacío", nameof(base64Content));
            if (string.IsNullOrWhiteSpace(targetPdfPath)) throw new ArgumentException("Ruta de archivo destino inválida", nameof(targetPdfPath));
            if (!File.Exists(targetPdfPath)) throw new FileNotFoundException("PDF base no encontrado", targetPdfPath);

            var dir = Path.GetDirectoryName(targetPdfPath)!;
            Directory.CreateDirectory(dir);

            // Normaliza base64
            var commaIndex = base64Content.IndexOf(',');
            if (base64Content.StartsWith("data:") && commaIndex > 0)
            {
                base64Content = base64Content[(commaIndex + 1)..];
            }

            byte[] newPdfBytes;
            try
            {
                newPdfBytes = Convert.FromBase64String(base64Content);
            }
            catch (FormatException)
            {
                throw new ArgumentException("El contenido no es un base64 válido", nameof(base64Content));
            }

            // Escribe la parte nueva a un archivo temporal
            var tempNew = Path.Combine(dir, $"append_{Guid.NewGuid():N}.pdf");
            var tempOut = Path.Combine(dir, $"merged_{Guid.NewGuid():N}.pdf");
            await File.WriteAllBytesAsync(tempNew, newPdfBytes);

            try
            {
                if (!TryConcatWithGhostscript(targetPdfPath, tempNew, tempOut, gsCmd))
                {
                    if (!TryConcatWithPdfUnite(targetPdfPath, tempNew, tempOut))
                    {
                        if (!TryConcatWithQpdf(targetPdfPath, tempNew, tempOut))
                        {
                            throw new InvalidOperationException("No se pudo concatenar PDFs: no hay herramienta disponible (gs/pdfunite/qpdf)");
                        }
                    }
                }

                // Reemplaza el archivo original por el combinado
                var backup = targetPdfPath + ".bak";
                if (File.Exists(backup)) File.Delete(backup);
                File.Move(targetPdfPath, backup);
                File.Move(tempOut, targetPdfPath);
                File.Delete(backup);
            }
            finally
            {
                // Limpieza de temporales
                try { if (File.Exists(tempNew)) File.Delete(tempNew); } catch { }
                try { if (File.Exists(tempOut)) File.Delete(tempOut); } catch { }
            }
        }

        // Anexa un PDF (base64) al final de un PDF base, generando un NUEVO archivo de salida y preservando el original.
        // Devuelve el path final de salida.
        public async Task<string> AppendPdfBase64ToNewFileAsync(string base64Content, string basePdfPath, string outPdfPath)
        {
            if (string.IsNullOrWhiteSpace(base64Content)) throw new ArgumentException("Contenido base64 vacío", nameof(base64Content));
            if (string.IsNullOrWhiteSpace(basePdfPath)) throw new ArgumentException("Ruta de PDF base inválida", nameof(basePdfPath));
            if (!File.Exists(basePdfPath)) throw new FileNotFoundException("PDF base no encontrado", basePdfPath);

            var dir = Path.GetDirectoryName(basePdfPath)!;
            Directory.CreateDirectory(dir);

            // Normaliza base64
            var commaIndex = base64Content.IndexOf(',');
            if (base64Content.StartsWith("data:") && commaIndex > 0)
            {
                base64Content = base64Content[(commaIndex + 1)..];
            }

            byte[] newPdfBytes;
            try
            {
                newPdfBytes = Convert.FromBase64String(base64Content);
            }
            catch (FormatException)
            {
                throw new ArgumentException("El contenido no es un base64 válido", nameof(base64Content));
            }

            // Asegura nombre único si el de salida existe
            var outputDir = Path.GetDirectoryName(outPdfPath)!;
            Directory.CreateDirectory(outputDir);
            var outputName = Path.GetFileName(outPdfPath);
            var outBase = Path.GetFileNameWithoutExtension(outputName);
            var outExt = Path.GetExtension(outputName);
            var candidateOut = outPdfPath;
            int counter = 1;
            while (File.Exists(candidateOut))
            {
                candidateOut = Path.Combine(outputDir, $"{outBase}_{counter}{outExt}");
                counter++;
            }

            // Escribe la parte nueva a un archivo temporal y concatena
            var tempNew = Path.Combine(outputDir, $"append_{Guid.NewGuid():N}.pdf");
            var tempOut = Path.Combine(outputDir, $"merged_{Guid.NewGuid():N}.pdf");
            await File.WriteAllBytesAsync(tempNew, newPdfBytes);

            try
            {
                if (!TryConcatWithGhostscript(basePdfPath, tempNew, tempOut, gsCmd))
                {
                    if (!TryConcatWithPdfUnite(basePdfPath, tempNew, tempOut))
                    {
                        if (!TryConcatWithQpdf(basePdfPath, tempNew, tempOut))
                        {
                            throw new InvalidOperationException("No se pudo concatenar PDFs: no hay herramienta disponible (gs/pdfunite/qpdf)");
                        }
                    }
                }

                // Mueve el resultado al archivo definitivo (sin tocar el original)
                File.Move(tempOut, candidateOut);

                try
                {
                    var baseName = Path.GetFileNameWithoutExtension(candidateOut);
                    var ext = (Path.GetExtension(candidateOut) ?? string.Empty).ToLowerInvariant();
                    if (ext == ".pdf")
                    {
                        if (!TryGeneratePdfThumbWithPdftoppm(candidateOut, outputDir, baseName, pdftoppmCmd))
                        {
                            TryGeneratePdfThumbWithGhostscript(candidateOut, outputDir, baseName, gsCmd);
                        }
                    }
                }
                catch { }

                return candidateOut;
            }
            finally
            {
                try { if (File.Exists(tempNew)) File.Delete(tempNew); } catch { }
                try { if (File.Exists(tempOut)) File.Delete(tempOut); } catch { }
            }
        }

        private static bool TryConcatWithGhostscript(string basePdf, string appendPdf, string outPdf, string gsExecutable)
        {
            try
            {
                var p = new Process();
                p.StartInfo.FileName = gsExecutable;
                p.StartInfo.ArgumentList.Add("-dBATCH");
                p.StartInfo.ArgumentList.Add("-dNOPAUSE");
                p.StartInfo.ArgumentList.Add("-q");
                p.StartInfo.ArgumentList.Add("-sDEVICE=pdfwrite");
                p.StartInfo.ArgumentList.Add("-sOutputFile=" + outPdf);
                p.StartInfo.ArgumentList.Add(basePdf);
                p.StartInfo.ArgumentList.Add(appendPdf);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                if (!p.WaitForExit(30000))
                {
                    try { p.Kill(entireProcessTree: true); } catch { }
                    return false;
                }
                return File.Exists(outPdf);
            }
            catch { return false; }
        }

        private static bool TryConcatWithPdfUnite(string basePdf, string appendPdf, string outPdf)
        {
            try
            {
                var p = new Process();
                p.StartInfo.FileName = "pdfunite";
                p.StartInfo.ArgumentList.Add(basePdf);
                p.StartInfo.ArgumentList.Add(appendPdf);
                p.StartInfo.ArgumentList.Add(outPdf);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                if (!p.WaitForExit(30000))
                {
                    try { p.Kill(entireProcessTree: true); } catch { }
                    return false;
                }
                return File.Exists(outPdf);
            }
            catch { return false; }
        }

        private static bool TryConcatWithQpdf(string basePdf, string appendPdf, string outPdf)
        {
            try
            {
                var p = new Process();
                p.StartInfo.FileName = "qpdf";
                p.StartInfo.ArgumentList.Add("--empty");
                p.StartInfo.ArgumentList.Add("--pages");
                p.StartInfo.ArgumentList.Add(basePdf);
                p.StartInfo.ArgumentList.Add(appendPdf);
                p.StartInfo.ArgumentList.Add("--");
                p.StartInfo.ArgumentList.Add(outPdf);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                if (!p.WaitForExit(30000))
                {
                    try { p.Kill(entireProcessTree: true); } catch { }
                    return false;
                }
                return File.Exists(outPdf);
            }
            catch { return false; }
        }

        private static bool TryGeneratePdfThumbWithPdftoppm(string pdfPath, string outputDir, string baseName, string pdftoppmExecutable)
        {
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = pdftoppmExecutable;
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
                if (!process.WaitForExit(30000))
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

        private static bool TryGeneratePdfThumbWithGhostscript(string pdfPath, string outputDir, string baseName, string gsExecutable)
        {
            try
            {
                var thumb = System.IO.Path.Combine(outputDir, $"{baseName}_thumb.png");
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = gsExecutable;
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
                if (!process.WaitForExit(30000))
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
