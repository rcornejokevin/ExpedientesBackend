using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
namespace DBHandler.Service.Catalog
{
    public class CasesNoteService
    {
        private readonly DBHandlerContext _context;
        private readonly EtapaService etapaService;
        private readonly EtapaDetalleService etapaDetalleService;

        public CasesNoteService(DBHandlerContext context)
        {
            _context = context;
            etapaService = new EtapaService(context);
            etapaDetalleService = new EtapaDetalleService(context);
        }

        public async Task<ExpedienteNotas> AddAsync(ExpedienteNotas expediente)
        {
            _context.ExpedienteNotas.Add(expediente);
            await _context.SaveChangesAsync();
            return expediente;
        }
        public async Task<List<ExpedienteNotas>> GetAllByExpedienteAsync(Expediente expediente)
        {
            return await _context.ExpedienteNotas
                .Where(u => u.ExpedienteId == expediente.Id)
                .ToListAsync();
        }
    }
}
