using DBHandler.Models;
using DBHandler.Service.Catalog;

namespace BusinessLogic.Services
{
    public class CasesDetailLogic
    {
        public CasesService casesService;
        public CasesDetailLogic(CasesService casesService)
        {
            this.casesService = casesService;
        }

    }
}