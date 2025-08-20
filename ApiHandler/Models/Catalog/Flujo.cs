using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Catalog
{
    public class NewFlujoRequest
    {
        [Required]
        public string nombre { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
    }
    public class EditFlujoRequest
    {
        [Range(1, int.MaxValue)]
        public int id { get; set; }
        [Required]
        public string nombre { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
    }
}