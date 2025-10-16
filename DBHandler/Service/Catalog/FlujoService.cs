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
                .Where(u => u.Activo == 1)
                .ToListAsync();
        }
        public async Task<Flujo?> GetByIdAsync(int id)
        {
            return await dbContext.Flujos
                .Where(u => u.Id == id)
                .Where(u => u.Activo == 1)
                .FirstOrDefaultAsync();
        }
        public async Task<Flujo?> ExistsByCorrelativoAsync(string correlativo)
        {
            return await dbContext.Flujos
                .Where(u => u.Correlativo == correlativo)
                .FirstOrDefaultAsync();
        }
        public async Task<Flujo?> ExistsByNombreAsync(string nombre)
        {
            return await dbContext.Flujos
                .Where(u => u.Nombre == nombre)
                .FirstOrDefaultAsync();
        }
        public async Task<Flujo> AddAsync(Flujo flujo)
        {

            if (ExistsByNombreAsync(flujo.Nombre).Result != null)
            {
                throw new Exception("Ya existe un flujo con el nombre: " + flujo.Nombre);
            }
            if (ExistsByCorrelativoAsync(flujo.Correlativo).Result != null)
            {
                throw new Exception("Ya existe un flujo con el correlativo: " + flujo.Correlativo);
            }
            dbContext.Flujos.Add(flujo);
            await dbContext.SaveChangesAsync();
            return flujo;
        }
        public async Task<Flujo> EditAsync(Flujo flujo)
        {
            Flujo? existingByName = ExistsByNombreAsync(flujo.Nombre).Result;
            if (existingByName != null && existingByName.Id != flujo.Id)
            {
                throw new Exception("Ya existe un flujo con el nombre: " + flujo.Nombre);
            }
            Flujo? existingByCorrelativo = ExistsByCorrelativoAsync(flujo.Correlativo).Result;
            if (existingByCorrelativo != null && existingByCorrelativo.Id != flujo.Id)
            {
                throw new Exception("Ya existe un flujo con el correlativo: " + flujo.Correlativo);
            }
            dbContext.Flujos.Update(flujo);
            await dbContext.SaveChangesAsync();
            return flujo;
        }
    }
}