
using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Catalog
{
    public class NewEtapaDetalleRequest
    {
        [Required]
        public int etapaId { get; set; }
        public string nombre { get; set; } = string.Empty;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Orden debe ser mayor a cero")]
        public int orden { get; set; }
        public string detalle { get; set; } = string.Empty;
    }
    public class EditEtapaDetalleRequest
    {
        [Required]
        public int id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Ingrese Etapa Id")]
        public int etapaId { get; set; }
        [Required]
        public string nombre { get; set; } = string.Empty;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Orden debe ser mayor a cero")]
        public int orden { get; set; }
        public string detalle { get; set; } = string.Empty;
    }
}