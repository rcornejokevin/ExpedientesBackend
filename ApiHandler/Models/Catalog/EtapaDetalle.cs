
using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Catalog
{
    public class NewEtapaDetalleRequest
    {
        [Required(ErrorMessage = "Etapa id es requerido")]
        public int etapaId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "Orden debe ser mayor a 0")]
        [Range(1, int.MaxValue, ErrorMessage = "Orden debe ser mayor a cero")]
        public int orden { get; set; }
        public string detalle { get; set; } = string.Empty;
    }
    public class EditEtapaDetalleRequest
    {
        [Required(ErrorMessage = "El id es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El id es requerido")]
        public int id { get; set; }
        [Required(ErrorMessage = "Etapa id es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Ingrese Etapa Id")]
        public int etapaId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "Orden debe ser mayor a 0")]
        [Range(1, int.MaxValue, ErrorMessage = "Orden debe ser mayor a cero")]
        public int orden { get; set; }
        public string detalle { get; set; } = string.Empty;
    }
}