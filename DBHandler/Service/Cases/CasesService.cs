using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
namespace DBHandler.Service.Catalog
{
    public class CasesService
    {
        private readonly DBHandlerContext _context;
        private readonly EtapaService etapaService;
        private readonly EtapaDetalleService etapaDetalleService;

        public CasesService(DBHandlerContext context)
        {
            _context = context;
            etapaService = new EtapaService(context);
            etapaDetalleService = new EtapaDetalleService(context);
        }
        public async Task<List<Expediente>> GetAllAsync()
        {
            return await _context.Expedientes
                .Where(u => u.Activo)
                .ToListAsync();
        }
        public async Task<Expediente?> GetByIdAsync(int id)
        {
            return await _context.Expedientes
                .Where(u => u.Id == id)
                .Where(u => u.Activo)
                .FirstOrDefaultAsync();
        }
        public async Task<Expediente> AddAsync(Expediente expediente)
        {
            _context.Expedientes.Add(expediente);
            await _context.SaveChangesAsync();
            return expediente;
        }
    }
}