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
}