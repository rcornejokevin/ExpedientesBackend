using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
using System.Security.Cryptography;
using System.Text;

namespace DBHandler.Service.Security
{
    public class AuthService
    {
        private readonly LoginDbContext dbContextLogin;
        private readonly DBHandlerContext dbContext;
        private readonly UsuarioService usuarioService;

        public AuthService(LoginDbContext _dbContextLogin, DBHandlerContext _dbContext, UsuarioService _usuarioService)
        {
            dbContextLogin = _dbContextLogin;
            dbContext = _dbContext;
            usuarioService = _usuarioService;
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
    }
}
