using DBHandler.Models;
using DBHandler.Service.Catalog;

namespace BusinessLogic.Services
{
    public class CasesDetailLogic
    {
        public CasesService casesService;
        public ExpedienteDetalleService expedienteDetalleService;
        public CasesDetailLogic(CasesService casesService)
        {
            this.casesService = casesService;
        }
        public async Task addDetailByExpediente(Expediente expediente, string detail)
        {

        }

    }
}