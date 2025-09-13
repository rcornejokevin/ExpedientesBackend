using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Security
{
    public class AddRemitenteRequest
    {
        [Required(ErrorMessage = "La descripcion es requerida")]
        public string descripcion { get; set; } = String.Empty;
    }
    public class EditRemitenteRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id requerido")]
        public int id { get; set; }
        [Required(ErrorMessage = "La descripcion es requerida")]
        public string descripcion { get; set; } = String.Empty;
    }
}