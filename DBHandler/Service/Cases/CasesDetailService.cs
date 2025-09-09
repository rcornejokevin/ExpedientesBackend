using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
namespace DBHandler.Service.Cases
{
    public class CasesDetailService
    {
        private readonly DBHandlerContext context;
        public CasesDetailService(DBHandlerContext context)
        {
            this.context = context;
        }
        public async Task<ExpedienteDetalle> AddAsync(ExpedienteDetalle expedienteDetalle)
        {
            context.ExpedienteDetalles.Add(expedienteDetalle);
            await context.SaveChangesAsync();
            return expedienteDetalle;
        }
        public async Task<ExpedienteDetalle> EditAsync(ExpedienteDetalle expedienteDetalle)
        {
            context.ExpedienteDetalles.Update(expedienteDetalle);
            await context.SaveChangesAsync();
            return expedienteDetalle;
        }

        public async Task<ExpedienteDetalle?> GetByIdAsync(int id)
        {
            return await context.ExpedienteDetalles
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<List<ExpedienteDetalle>> GetAllByExpedienteAsync(Expediente expediente)
        {
            return await context.ExpedienteDetalles
                .Where(u => u.ExpedienteId == expediente.Id)
                .ToListAsync();
        }
        public async Task<List<ExpedienteDetalle>> GetAllByExpedienteIdAsync(int expedienteId)
        {
            return await context.ExpedienteDetalles
                .Where(u => u.ExpedienteId == expedienteId)
                .ToListAsync();
        }
        public async Task<List<ExpedienteDetalle>> GetAllAsync()
        {
            return await context.ExpedienteDetalles
                .ToListAsync();
        }
    }
}