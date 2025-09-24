using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<int> CountByExpedienteIdAsync(int expedienteId)
        {
            return await context.ExpedienteDetalles
                .Where(u => u.ExpedienteId == expedienteId)
                .Where(u => u.Ubicacion != null && u.Ubicacion != "")
                .CountAsync();
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
            var relatedExpedienteIds = await GetRelatedExpedienteIdsAsync(expedienteId);

            var idsToQuery = relatedExpedienteIds.Any()
                ? relatedExpedienteIds.ToList()
                : new List<int> { expedienteId };

            return await context.ExpedienteDetalles
                .Where(u => idsToQuery.Contains(u.ExpedienteId))
                .OrderBy(u => u.Fecha)
                .ToListAsync();
        }
        public async Task<List<ExpedienteDetalle>> GetAllAsync()
        {
            return await context.ExpedienteDetalles
                .ToListAsync();
        }

        public async Task<List<ExpedienteDetalle>> GetAllByDateRangeWithEtapaAsync(DateTime startInclusive, DateTime endExclusive)
        {
            return await context.ExpedienteDetalles
                .Where(d => d.Fecha >= startInclusive && d.Fecha < endExclusive)
                .Include(d => d.EtapaNueva)
                .ThenInclude(e => e.Flujo)
                .ToListAsync();
        }

        private async Task<HashSet<int>> GetRelatedExpedienteIdsAsync(int expedienteId)
        {
            var relatedIds = new HashSet<int>();
            var toVisit = new Queue<int>();
            toVisit.Enqueue(expedienteId);

            // Traverse the relationship graph to gather every expediente linked to the requested one.
            while (toVisit.Count > 0)
            {
                var currentId = toVisit.Dequeue();
                if (!relatedIds.Add(currentId))
                {
                    continue;
                }

                var relatedExpedientes = await context.Expedientes
                    .AsNoTracking()
                    .Where(e => e.Id == currentId ||
                                (e.ExpedienteRelacionadoId.HasValue && e.ExpedienteRelacionadoId.Value > 0 && e.ExpedienteRelacionadoId.Value == currentId))
                    .Select(e => new { e.Id, e.ExpedienteRelacionadoId })
                    .ToListAsync();

                if (relatedExpedientes.Count == 0)
                {
                    continue;
                }

                var current = relatedExpedientes.FirstOrDefault(e => e.Id == currentId);
                if (current != null && current.ExpedienteRelacionadoId.HasValue && current.ExpedienteRelacionadoId.Value > 0)
                {
                    toVisit.Enqueue(current.ExpedienteRelacionadoId.Value);
                }

                foreach (var expediente in relatedExpedientes)
                {
                    if (expediente.Id != currentId)
                    {
                        toVisit.Enqueue(expediente.Id);
                    }
                }
            }

            return relatedIds;
        }
    }
}
