using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;

namespace DBHandler.Service.Security
{
    public class UsuarioService
    {
        private readonly DBHandlerContext _dbContext;

        public UsuarioService(DBHandlerContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Usuario> createAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Add(usuario);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
            return usuario;
        }
        public async Task<Usuario> updateAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Update(usuario);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
            return usuario;
        }
        public async Task deleteAsync(Usuario usuario)
        {
            usuario.Activo = 0;
            _dbContext.Usuarios.Update(usuario);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _dbContext.Usuarios
                .Where(u => u.Activo == 1)
                .ToListAsync();
        }
        public async Task<Usuario?> getUsuarioByUsername(string? username)
        {
            return await _dbContext.Usuarios
                .Where(u => u.Activo == 1)
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();
        }
        public async Task<Usuario?> getUsuarioByIdAsync(int? id)
        {
            return await _dbContext.Usuarios
                 .Where(u => u.Activo == 1)
                 .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
