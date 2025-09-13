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
        public async Task<List<Expediente>> GetAllAsync(Filters filters)
        {
            IQueryable<Expediente> query = _context.Expedientes.AsQueryable();

            query = query.Where(e => e.Activo);

            if (filters.Usuario.HasValue && filters.Usuario.Value > 0)
            {
                query = query.Where(e => e.AsesorId == filters.Usuario.Value);
            }
            else if (filters.AsesorId.HasValue && filters.AsesorId.Value > 0)
            {
                query = query.Where(e => e.AsesorId == filters.AsesorId.Value);
            }

            if (filters.FlujoId.HasValue && filters.FlujoId.Value > 0)
            {
                query = query.Where(e => e.Etapa.FlujoId == filters.FlujoId.Value);
            }

            if (filters.EtapaId.HasValue && filters.EtapaId.Value > 0)
            {
                query = query.Where(e => e.EtapaId == filters.EtapaId.Value);
            }
            if (filters.SubEtapaId.HasValue && filters.SubEtapaId.Value > 0)
            {
                query = query.Where(e => e.EtapaDetalleId == filters.SubEtapaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Estatus))
            {
                query = query.Where(e => e.Estatus == filters.Estatus);
            }

            if (!string.IsNullOrWhiteSpace(filters.Asunto))
            {
                var term = filters.Asunto.Trim().ToUpper();
                query = query.Where(e => e.Asunto.ToUpper().Contains(term));
            }

            if (filters.RemitenteId.HasValue && filters.RemitenteId.Value > 0)
            {
                query = query.Where(e => e.RemitenteId == filters.RemitenteId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.FechaInicioIngreso))
            {
                if (DateTime.TryParse(filters.FechaInicioIngreso, out var fi))
                {
                    query = query.Where(e => e.FechaIngreso >= fi.Date);
                }
            }
            if (!string.IsNullOrWhiteSpace(filters.FechaFinIngreso))
            {
                if (DateTime.TryParse(filters.FechaFinIngreso, out var ff))
                {
                    var end = ff.Date.AddDays(1); // exclusivo
                    query = query.Where(e => e.FechaIngreso < end);
                }
            }

            if (!string.IsNullOrWhiteSpace(filters.FechaInicioActualizacion))
            {
                if (DateTime.TryParse(filters.FechaInicioActualizacion, out var fai))
                {
                    query = query.Where(e => e.FechaActualizacion >= fai.Date);
                }
            }
            if (!string.IsNullOrWhiteSpace(filters.FechaFinActualizacion))
            {
                if (DateTime.TryParse(filters.FechaFinActualizacion, out var faf))
                {
                    var end = faf.Date.AddDays(1);
                    query = query.Where(e => e.FechaActualizacion < end);
                }
            }

            query = query
                .Include(e => e.Etapa)
                    .ThenInclude(et => et.Flujo)
                .Include(e => e.EtapaDetalle)
                .Include(e => e.Usuario)
                .Include(e => e.Remitente)
                .AsQueryable();

            query = query.OrderByDescending(e => e.FechaActualizacion ?? e.FechaIngreso);

            if (filters.Limit.HasValue && filters.Limit.Value > 0)
            {
                query = query.Take(filters.Limit.Value);
            }

            return await query.ToListAsync();
        }
        public async Task<List<Expediente>> GetAllByUserAsync(Usuario user)
        {
            return await _context.Expedientes
                .Where(u => u.Activo && u.AsesorId == user.Id)
                .Include(e => e.Etapa)
                .ToListAsync();
        }
        public async Task<Expediente?> GetByIdAsync(int id)
        {
            return await _context.Expedientes
                .Where(u => u.Id == id)
                .Where(u => u.Activo)
                .Include(e => e.Etapa)
                .FirstOrDefaultAsync();
        }
        public async Task<Expediente> AddAsync(Expediente expediente)
        {
            _context.Expedientes.Add(expediente);
            await _context.SaveChangesAsync();
            return expediente;
        }
        public async Task<Expediente> EditAsync(Expediente expediente)
        {
            _context.Expedientes.Update(expediente);
            await _context.SaveChangesAsync();
            return expediente;
        }
    }
}
