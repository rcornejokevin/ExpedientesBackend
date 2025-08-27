using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
using DBHandler.Exceptions;
namespace DBHandler.Service.Catalog
{
    public class EtapaService
    {
        private readonly DBHandlerContext dbContext;
        private readonly FlujoService flujoService;

        public EtapaService(DBHandlerContext _dbContext)
        {
            dbContext = _dbContext;
            flujoService = new FlujoService(dbContext);
        }
        public async Task<List<Etapa>> GetAllAsync()
        {
            return await dbContext.Etapas
                .Where(u => u.Activo)
                .OrderBy(u => u.Orden)
                .ToListAsync();
        }
        public async Task<List<Etapa>> GetAllByFlujoIdAsync(int flujoId)
        {
            return await dbContext.Etapas
                .Where(u => u.FlujoId == flujoId)
                .OrderBy(u => u.Orden)
                .ToListAsync();
        }
        public async Task<Etapa> AddAsync(Etapa etapa)
        {

            Flujo? flujo = await flujoService.GetByIdAsync(etapa.FlujoId);
            if (flujo == null)
            {
                throw new NotFoundException("Flujo no encontrado");
            }
            dbContext.Etapas.Add(etapa);
            await dbContext.SaveChangesAsync();
            return etapa;
        }
        public async Task<Etapa> EditAsync(Etapa etapa)
        {
            Flujo? flujo = await flujoService.GetByIdAsync(etapa.FlujoId);
            if (flujo == null)
            {
                throw new NotFoundException("Flujo no encontrado");
            }
            dbContext.Etapas.Update(etapa);
            await dbContext.SaveChangesAsync();
            return etapa;
        }
        public async Task<Etapa?> GetEtapaByIdAsync(int id)
        {
            return await dbContext.Etapas
                .Where(u => u.Id == id)
                .Where(u => u.Activo)
                .FirstOrDefaultAsync();
        }
    }
}