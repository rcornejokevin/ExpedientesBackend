using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
using DBHandler.Exceptions;
namespace DBHandler.Service.Catalog
{
    public class EtapaDetalleService
    {
        private readonly DBHandlerContext dbContext;
        private readonly EtapaService etapaService;

        public EtapaDetalleService(DBHandlerContext _dbContext)
        {
            dbContext = _dbContext;
            etapaService = new EtapaService(dbContext);
        }
        public async Task<List<EtapaDetalle>> GetAllAsync()
        {
            return await dbContext.EtapaDetalles
                .Where(u => u.Activo == 1)
                .OrderBy(u => u.Orden)
                .ToListAsync();
        }

        public async Task<List<EtapaDetalle>> GetAllByEtapaAsync(Etapa etapa)
        {
            return await dbContext.EtapaDetalles
                .Where(u => u.Activo == 1)
                .Where(u => u.EtapaId == etapa.Id)
                .ToListAsync();
        }
        public async Task<EtapaDetalle> AddAsync(EtapaDetalle etapaDetalle)
        {
            Etapa? etapa = await etapaService.GetEtapaByIdAsync(etapaDetalle.EtapaId);
            if (etapa == null)
            {
                throw new NotFoundException("Etapa no encontrada");
            }
            dbContext.EtapaDetalles.Add(etapaDetalle);
            await dbContext.SaveChangesAsync();
            return etapaDetalle;
        }
        public async Task<EtapaDetalle> EditAsync(EtapaDetalle etapaDetalle)
        {
            Etapa? etapa = await etapaService.GetEtapaByIdAsync(etapaDetalle.EtapaId);
            if (etapa == null)
            {
                throw new NotFoundException("Etapa no encontrada");
            }
            dbContext.EtapaDetalles.Update(etapaDetalle);
            await dbContext.SaveChangesAsync();
            return etapaDetalle;
        }
        public async Task<EtapaDetalle?> GetByIdAsync(int id)
        {
            return await dbContext.EtapaDetalles
                .Where(u => u.Id == id)
                .Where(u => u.Activo == 1)
                .FirstOrDefaultAsync();
        }
        public async Task<EtapaDetalle?> getFirstEtapaDetalleByEtapaId(int etapaId)
        {
            return await dbContext.EtapaDetalles
                .Where(u => u.Activo == 1)
                .Where(u => u.EtapaId == etapaId)
                .OrderBy(u => u.Orden)
                .FirstOrDefaultAsync();
        }
    }
}