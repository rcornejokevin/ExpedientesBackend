using DBHandler.Service.Catalog;
using CampoDB = DBHandler.Models.Campo;
using DBHandler.Models;
namespace BusinessLogic.Services
{
    public class CampoLogic
    {
        public CampoService campoService;
        public EtapaService etapaService;
        public EtapaDetalleService etapaDetalleService;

        public CampoLogic(CampoService campoService, EtapaService etapaService, EtapaDetalleService etapaDetalleService)
        {
            this.campoService = campoService;
            this.etapaService = etapaService;
            this.etapaDetalleService = etapaDetalleService;
        }
        public async Task<CampoDB> addCampo(CampoDB campo)
        {
            if (await checkToSave(campo) == true) campo = await campoService.AddAsync(campo);
            return campo;
        }

        public async Task<CampoDB> editCampo(CampoDB campo)
        {
            if (await checkToSave(campo) == true) campo = await campoService.EditAsync(campo);
            return campo;
        }
        public async Task<bool> checkToSave(CampoDB campo)
        {
            if (campo.EtapaId == 0 && campo.EtapaDetalleId == 0)
            {
                throw new ArgumentException("Campo debe tener una etapa o subetapa asignada.");
            }
            if (campo.EtapaId != 0)
            {
                Etapa? etapa = await etapaService.GetEtapaByIdAsync(campo.EtapaId ?? 0);
                if (etapa == null)
                {
                    throw new KeyNotFoundException("Etapa no encontrada.");
                }
            }
            if (campo.EtapaDetalleId != 0)
            {
                Etapa? etapa = await etapaService.GetEtapaByIdAsync(campo.EtapaId ?? 0);
                if (etapa == null)
                {
                    throw new KeyNotFoundException("Sub Etapa no encontrada.");
                }
            }
            return true;
        }
    }
}