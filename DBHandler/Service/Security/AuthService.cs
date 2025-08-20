using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;

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
            return await dbContextLogin.UsuariosNidas.FirstOrDefaultAsync(
                u => u.NombreUsuario == username && u.Clave == password
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
    }
}