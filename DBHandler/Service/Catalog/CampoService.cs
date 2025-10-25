using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using DBHandler.Models;
namespace DBHandler.Service.Catalog
{
    public class CampoService
    {
        private readonly DBHandlerContext _context;
        public CampoService(DBHandlerContext context)
        {
            _context = context;
        }
        public async Task<List<Campo>> GetAllAsync()
        {
            return await _context.Campos
                .Where(u => u.Activo == 1)
                .OrderBy(u => u.Orden)
                .ToListAsync();
        }
        public async Task<Campo?> GetByIdAsync(int id)
        {
            return await _context.Campos
                .Where(u => u.Id == id)
                .Where(u => u.Activo == 1)
                .FirstOrDefaultAsync();
        }
        public async Task<Campo> AddAsync(Campo campo)
        {
            if (await ExistsByNombreAndFlujoIdAsync(campo.Nombre, campo.FlujoId ?? 0) != null)
            {
                throw new Exception("Ya existe un campo con el nombre: " + campo.Nombre);
            }
            _context.Campos.Add(campo);
            await _context.SaveChangesAsync();
            return campo;
        }
        public async Task<Campo> EditAsync(Campo campo)
        {
            Campo? existingCampo = await ExistsByNombreAndFlujoIdAsync(campo.Nombre, campo.FlujoId ?? 0);
            if (existingCampo != null && existingCampo.Id != campo.Id)
            {
                throw new Exception("Ya existe un campo con el nombre: " + campo.Nombre);
            }
            _context.Campos.Update(campo);
            await _context.SaveChangesAsync();
            return campo;
        }
        public async Task<Campo?> ExistsByNombreAndFlujoIdAsync(string nombre, int flujoId)
        {
            return await _context.Campos
                .Where(u => u.Nombre == nombre)
                .Where(u => u.FlujoId == flujoId)
                .FirstOrDefaultAsync();
        }
    }
}