using System.ComponentModel.DataAnnotations;
using ApiHandler.Models.Validations;

namespace ApiHandler.Models.Catalog
{
    public class NewFlujoRequest
    {
        [Required(ErrorMessage = "El nombre del flujo es requerido")]
        [UniqueNombre(ErrorMessage = "El nombre debe ser único.")]
        public string nombre { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
    }
    public class EditFlujoRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "El Id es obligatorio")]
        public int id { get; set; }
        [Required(ErrorMessage = "El nombre del flujo es requerido")]
        [UniqueNombre(nameof(id), ErrorMessage = "El nombre debe ser único.")]
        public string nombre { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
    }
}