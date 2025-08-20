using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
namespace DBHandler.Service.Catalog
{
    public class FlujoService
    {
        private readonly DBHandlerContext dbContext;
        public FlujoService(DBHandlerContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<List<Flujo>> GetAllAsync()
        {
            return await dbContext.Flujos
                .Where(u => u.Activo)
                .ToListAsync();
        }
        public async Task<Flujo?> GetByIdAsync(int id)
        {
            return await dbContext.Flujos
                .Where(u => u.Id == id)
                .Where(u => u.Activo)
                .FirstOrDefaultAsync();
        }
        public async Task<Flujo> AddAsync(Flujo flujo)
        {
            dbContext.Flujos.Add(flujo);
            await dbContext.SaveChangesAsync();
            return flujo;
        }
        public async Task<Flujo> EditAsync(Flujo flujo)
        {
            dbContext.Flujos.Update(flujo);
            await dbContext.SaveChangesAsync();
            return flujo;
        }
    }
}