using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;

namespace DBHandler.Service.Catalog
{
    public class RemitenteService
    {
        private readonly DBHandlerContext _dbContext;

        public RemitenteService(DBHandlerContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Remitente> createAsync(Remitente remitente)
        {
            _dbContext.Remitentes.Add(remitente);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
            return remitente;
        }
        public async Task<Remitente> updateAsync(Remitente remitente)
        {
            _dbContext.Remitentes.Update(remitente);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
            return remitente;
        }
        public async Task deleteAsync(Remitente remitente)
        {
            remitente.Activo = 0;
            _dbContext.Remitentes.Update(remitente);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Remitente>> GetAllAsync()
        {
            return await _dbContext.Remitentes
                .Where(u => u.Activo == 1)
                .ToListAsync();
        }
        public async Task<Remitente?> getRemitenteById(int? id)
        {
            return await _dbContext.Remitentes
                 .Where(u => u.Activo == 1)
                 .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}