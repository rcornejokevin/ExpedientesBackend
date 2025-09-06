using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiHandler.Models.Validations;
namespace ApiHandler.Models.Catalog
{
    public class NewCampoRequest
    {
        [Required]
        public string nombre { get; set; } = String.Empty;
        public int? etapaId { get; set; }
        public int flujoId { get; set; }
        [Required]
        [RegularExpression("^(Texto|Numero|Fecha|Opciones|Cheque)$", ErrorMessage = "Campo inválido")]
        public string tipoCampo { get; set; } = String.Empty;
        [Required]
        public int orden { get; set; }
        [Required]
        public bool requerido { get; set; }
        [Required]
        public string label { get; set; } = String.Empty;
        [Required]
        public string placeHolder { get; set; } = String.Empty;
        public string opciones { get; set; } = String.Empty;
    }
    public class EditCampoRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El id es requerido")]
        public int id { get; set; }
        [Required]
        public string nombre { get; set; } = String.Empty;

        public int? etapaId { get; set; }
        public int flujoId { get; set; }
        [Required]
        [RegularExpression("^(Texto|Numero|Fecha|Opciones|Cheque)$", ErrorMessage = "Campo inválido")]
        public string tipoCampo { get; set; } = String.Empty;
        [Required]
        public int orden { get; set; }
        [Required]
        public bool requerido { get; set; }
        [Required]
        public string label { get; set; } = String.Empty;
        [Required]
        public string placeHolder { get; set; } = String.Empty;
        public string opciones { get; set; } = String.Empty;
    }
}