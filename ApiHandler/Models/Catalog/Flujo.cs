using System.ComponentModel.DataAnnotations;
using ApiHandler.Models.Validations;

namespace ApiHandler.Models.Catalog
{
    public class NewFlujoRequest
    {
        [Required]
        [UniqueNombre(ErrorMessage = "El nombre debe ser único.")]
        public string nombre { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
    }
    public class EditFlujoRequest
    {
        [Range(1, int.MaxValue)]
        public int id { get; set; }
        [Required]
        [UniqueNombre(nameof(id), ErrorMessage = "El nombre debe ser único.")]
        public string nombre { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
    }
}