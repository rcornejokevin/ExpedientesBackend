using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Catalog
{
    public class NewEtapaRequest
    {
        [Required]
        public string nombre { get; set; } = String.Empty;
        [Required]
        public int orden { get; set; }
        public string detalle { get; set; } = String.Empty;
        [Required]
        public int flujoId { get; set; }
    }
    public class EditEtapaRequest
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string nombre { get; set; } = String.Empty;
        [Required]
        public int orden { get; set; }
        public string detalle { get; set; } = String.Empty;
        [Required]
        public int flujoId { get; set; }
    }
    public class OrdenDetail
    {
        [Required]
        public int id { get; set; }

        [Required]
        public int orden { get; set; }
    }

    public class EditOrdenEtapaRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "Debe enviar al menos un item.")]
        public List<OrdenDetail> items { get; set; } = new();
    }
}