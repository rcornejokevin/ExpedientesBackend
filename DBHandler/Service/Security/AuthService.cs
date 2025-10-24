using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace DBHandler.Service.Security
{
    public class AuthService
    {
        private readonly LoginDbContext dbContextLogin;
        private readonly DBHandlerContext dbContext;
        private readonly UsuarioService usuarioService;
        private readonly IConfiguration configuration;

        public AuthService(LoginDbContext _dbContextLogin, DBHandlerContext _dbContext, UsuarioService _usuarioService, IConfiguration configuration)
        {
            dbContextLogin = _dbContextLogin;
            dbContext = _dbContext;
            usuarioService = _usuarioService;
            this.configuration = configuration;
        }

        public async Task<UsuariosNida?> ExternalLoginAsync(string? username, string? password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            string passwordHash = ComputeMd5Hash(password);
            string passwordHashLower = passwordHash.ToLowerInvariant();

            return await dbContextLogin.UsuariosNidas.FirstOrDefaultAsync(
                u => u.NombreUsuario == username &&
                     (u.Clave == passwordHash || u.Clave == passwordHashLower)
            );
        }
        public async Task<UsuariosNida?> GetExternalUserByUsernameAsync(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            return await dbContextLogin.UsuariosNidas.FirstOrDefaultAsync(
                u => u.NombreUsuario == username
            );
        }
        public async Task<Usuario?> LoginSuccessAsync(string? username, string? password)
        {
            Usuario? usuarioLogin = await usuarioService.getUsuarioByUsername(username);
            if (usuarioLogin == null)
            {
                return null;
            }
            if (await this.ExternalLoginAsync(username, password) == null)
            {
                return null;
            }
            return usuarioLogin;
        }

        private static string ComputeMd5Hash(string value)
        {
            using MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(hashBytes);
        }
        public async Task<bool> ChangePasswordByUsernameAsync(string username, string newPassword)
        {
            UsuariosNida? usuario = await GetExternalUserByUsernameAsync(username);
            if (usuario == null)
            {
                return false;
            }

            string newPasswordHash = ComputeMd5Hash(newPassword);
            usuario.Clave = newPasswordHash;
            usuario.UltimoCambioClave = DateTime.UtcNow;
            dbContextLogin.UsuariosNidas.Update(usuario);
            await dbContextLogin.SaveChangesAsync();

            return true;
        }
        public bool canLoginByTerminal(Usuario usuario, string terminal)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(terminal))
            {
                return false;
            }

            if (usuario.Terminal == "*")
            {
                return true;
            }

            if (usuario.Terminal == terminal)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SendTwoFactorAsync(Usuario usuario, string terminal, CancellationToken cancellationToken = default)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email))
            {
                return false;
            }

            string username = usuario.Username;
            string terminalHash = GenerateTerminalRequestHash(usuario, terminal);

            IConfigurationSection emailSection = configuration.GetSection("EmailSettings");
            string? smtpHost = emailSection["Host"];
            string? smtpUser = emailSection["Username"];
            string? smtpPassword = emailSection["Password"];
            string? fromAddress = emailSection["From"];
            string? subject = emailSection["Subject"];
            string? bodyTemplate = emailSection["BodyTemplate"];
            string? actionUrlTemplate = emailSection["ActionUrl"];
            string? actionLink = BuildActionLink(actionUrlTemplate, terminalHash);

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException("Email configuration is incomplete. Please configure EmailSettings:Host and EmailSettings:From.");
            }
            bool enableSsl = true;
            if (bool.TryParse(emailSection["EnableSsl"], out bool enableSslConfig))
            {
                enableSsl = enableSslConfig;
            }
            bool isBodyHtml = false;
            if (bool.TryParse(emailSection["IsBodyHtml"], out bool isBodyHtmlConfig))
            {
                isBodyHtml = isBodyHtmlConfig;
            }
            int smtpPort = 587;
            if (int.TryParse(emailSection["Port"], out int portConfig))
            {
                smtpPort = portConfig;
            }
            string subjectToUse = string.IsNullOrWhiteSpace(subject) ? "Solicitud de validación de terminal" : subject;
            string bodyToUse = string.IsNullOrWhiteSpace(bodyTemplate)
                ? BuildDefaultBody(username, terminal, terminalHash, actionLink, isBodyHtml)
                : FormatBody(bodyTemplate, username, terminal, terminalHash, actionLink, isBodyHtml);

            using MailMessage message = new MailMessage
            {
                From = new MailAddress(fromAddress),
                Subject = subjectToUse,
                Body = bodyToUse,
                IsBodyHtml = isBodyHtml
            };
            message.To.Add(usuario.Email);
            using SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl
            };
            if (!string.IsNullOrWhiteSpace(smtpUser) && !string.IsNullOrWhiteSpace(smtpPassword))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
            }
            await smtpClient.SendMailAsync(message, cancellationToken);

            usuario.NuevaTerminal = ComposePendingTerminalValue(terminalHash, terminal);
            await usuarioService.updateAsync(usuario);

            return true;
        }

        public async Task<Usuario?> GetUsuarioByTerminalHashAsync(string terminalHash, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(terminalHash))
            {
                return null;
            }

            var usuarios = await dbContext.Usuarios
                .Where(u => u.Activo == 1 && !string.IsNullOrWhiteSpace(u.NuevaTerminal))
                .ToListAsync(cancellationToken);

            return usuarios.FirstOrDefault(u =>
                TryParsePendingTerminalValue(u.NuevaTerminal, out var storedHash, out _) &&
                string.Equals(storedHash, terminalHash, StringComparison.Ordinal));
        }

        public async Task<Usuario> ApprovePendingTerminalAsync(Usuario usuario, string? terminal, CancellationToken cancellationToken = default)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            string sanitizedTerminal = terminal?.Trim() ?? string.Empty;

            if (string.Equals(usuario.Terminal, "*", StringComparison.Ordinal))
            {
                usuario.NuevaTerminal = string.Empty;
                return await usuarioService.updateAsync(usuario);
            }
            usuario.Terminal = sanitizedTerminal == null ? "" : sanitizedTerminal;
            usuario.NuevaTerminal = "";
            return await usuarioService.updateAsync(usuario);
        }

        private string GetTwoFactorSecretKey()
        {
            string? secret = configuration["TwoFactor:SecretKey"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                secret = configuration["JwtSettings:SecretKey"];
            }

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("TwoFactor secret key is not configured. Please configure TwoFactor:SecretKey or reuse JwtSettings:SecretKey.");
            }

            return secret;
        }

        private string GenerateTerminalRequestHash(Usuario usuario, string terminal)
        {
            string secret = GetTwoFactorSecretKey();
            string payload = $"{usuario.Id}|{usuario.Username}|{terminal}|{DateTime.UtcNow:O}|{Guid.NewGuid()}";

            using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            return Convert.ToBase64String(hashBytes);
        }

        private static string? BuildActionLink(string? template, string hash)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return null;
            }

            string encodedHash = WebUtility.UrlEncode(hash);
            if (template.Contains("{hash}", StringComparison.OrdinalIgnoreCase))
            {
                return template.Replace("{hash}", encodedHash, StringComparison.OrdinalIgnoreCase);
            }

            return $"{template.TrimEnd('/')}/{encodedHash}";
        }

        private static string BuildDefaultBody(string username, string terminal, string hash, string? link, bool asHtml)
        {
            if (asHtml)
            {
                string safeTerminal = WebUtility.HtmlEncode(terminal);
                string safeUsername = WebUtility.HtmlEncode(username);
                string safeHash = WebUtility.HtmlEncode(hash);
                string? safeLink = link != null ? WebUtility.HtmlEncode(link) : null;

                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<p>Se ha solicitado acceso al sistema desde la terminal \"")
                    .Append(safeTerminal)
                    .Append("\" para el usuario ")
                    .Append(safeUsername)
                    .Append(".</p>");

                if (!string.IsNullOrWhiteSpace(safeLink))
                {
                    htmlBuilder.Append("<p>Autoriza esta terminal utilizando el siguiente enlace: ")
                        .Append("<a href=\"")
                        .Append(safeLink)
                        .Append("\">Autorizar terminal</a></p>")
                        .Append("<p>Enlace directo: <a href=\"")
                        .Append(safeLink)
                        .Append("\">")
                        .Append(safeLink)
                        .Append("</a></p>");
                }

                htmlBuilder.Append("<p>Si el enlace no funciona, utiliza el siguiente identificador de validación:</p>")
                    .Append("<p><code>")
                    .Append(safeHash)
                    .Append("</code></p>")
                    .Append("<p>Este mensaje se generó automáticamente. Si no solicitaste acceso, puedes ignorarlo.</p>");

                return htmlBuilder.ToString();
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Se ha solicitado acceso al sistema desde la terminal \"{terminal}\" para el usuario {username}.");
            if (!string.IsNullOrWhiteSpace(link))
            {
                builder.AppendLine();
                builder.AppendLine("Autoriza esta terminal utilizando el siguiente enlace:");
                builder.AppendLine(link);
            }
            builder.AppendLine();
            builder.AppendLine("Si el enlace no funciona, utiliza el siguiente identificador de validación:");
            builder.AppendLine(hash);
            builder.AppendLine();
            builder.AppendLine("Este mensaje se generó automáticamente. Si no solicitaste acceso, puedes ignorarlo.");

            return builder.ToString();
        }

        public bool TryExtractPendingTerminal(Usuario? usuario, out string terminalHash, out string terminal)
        {
            if (usuario == null)
            {
                terminalHash = string.Empty;
                terminal = string.Empty;
                return false;
            }

            return TryParsePendingTerminalValue(usuario.NuevaTerminal, out terminalHash, out terminal);
        }

        private static string ComposePendingTerminalValue(string hash, string terminal)
        {
            string encodedTerminal = Convert.ToBase64String(Encoding.UTF8.GetBytes(terminal));
            return $"{hash}:{encodedTerminal}";
        }

        private static bool TryParsePendingTerminalValue(string? storedValue, out string hash, out string terminal)
        {
            hash = string.Empty;
            terminal = string.Empty;

            if (string.IsNullOrWhiteSpace(storedValue))
            {
                return false;
            }

            string[] parts = storedValue.Split(':', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 1)
            {
                hash = parts[0];
                return !string.IsNullOrWhiteSpace(hash);
            }

            hash = parts[0];

            try
            {
                terminal = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));
            }
            catch (FormatException)
            {
                terminal = string.Empty;
            }

            return !string.IsNullOrWhiteSpace(hash);
        }

        private static string FormatBody(string template, string username, string terminal, string hash, string? link, bool asHtml)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }

            string formatted = template
                .Replace("{username}", username, StringComparison.OrdinalIgnoreCase)
                .Replace("{terminal}", terminal, StringComparison.OrdinalIgnoreCase)
                .Replace("{hash}", hash, StringComparison.OrdinalIgnoreCase)
                .Replace("{link}", link ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{datetime}", DateTime.UtcNow.ToString("u"), StringComparison.OrdinalIgnoreCase);

            try
            {
                formatted = string.Format(formatted, username, terminal, hash, link ?? string.Empty, DateTime.UtcNow);
            }
            catch (FormatException)
            {
            }

            if (asHtml)
            {
                formatted = formatted.Replace("\r\n", "\n").Replace("\n", "<br/>");
            }

            return formatted;
        }
    }
}
