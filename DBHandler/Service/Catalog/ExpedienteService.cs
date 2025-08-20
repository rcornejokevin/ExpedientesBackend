using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
namespace DBHandler.Service.Catalog
{
    public class ExpedienteService
    {
        private readonly DBHandlerContext context;
        public ExpedienteService(DBHandlerContext context)
        {
            this.context = context;
        }
        public async Task<List<Expediente>> GetAllAsync()
        {
            return await context.Expedientes
                .ToListAsync();
        }
        public async Task<Expediente?> GetByIdAsync(int id)
        {
            return await context.Expedientes
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<Expediente> AddAsync(Expediente expediente)
        {
            context.Expedientes.Add(expediente);
            await context.SaveChangesAsync();
            return expediente;
        }
        public async Task<Expediente> EditAsync(Expediente expediente)
        {
            context.Expedientes.Update(expediente);
            await context.SaveChangesAsync();
            return expediente;
        }
    }
}