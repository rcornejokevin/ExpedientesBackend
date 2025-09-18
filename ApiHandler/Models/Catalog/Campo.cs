using System.ComponentModel.DataAnnotations;
namespace ApiHandler.Models.Catalog
{
    public class NewCampoRequest
    {
        [Required(ErrorMessage = "El campo nombre es requerido")]
        public string nombre { get; set; } = String.Empty;
        public int? etapaId { get; set; }
        public int flujoId { get; set; }
        [Required(ErrorMessage = "El campo tipo de campo es requerido")]
        [RegularExpression("^(Texto|Numero|Fecha|Opciones|Cheque|Memo)$", ErrorMessage = "Campo inválido")]
        public string tipoCampo { get; set; } = String.Empty;
        [Required(ErrorMessage = "El orden es requerido")]
        public int orden { get; set; }
        [Required(ErrorMessage = "El campo \"requerido\" es requerido")]
        public bool requerido { get; set; }
        [Required(ErrorMessage = "El campo label es requerido")]
        public string label { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo placeHolder es requerido")]
        public string placeHolder { get; set; } = String.Empty;
        public string opciones { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo \"editable\" es requerido")]
        public bool editable { get; set; }
    }
    public class EditCampoRequest
    {
        [Required(ErrorMessage = "El id es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El id es requerido")]
        public int id { get; set; }
        [Required(ErrorMessage = "El campo nombre es requerido")]
        public string nombre { get; set; } = String.Empty;

        public int? etapaId { get; set; }
        [Required(ErrorMessage = "El campo flujo es requerido")]
        public int flujoId { get; set; }
        [Required(ErrorMessage = "El campo tipo de campo es requerido")]
        [RegularExpression("^(Texto|Numero|Fecha|Opciones|Cheque|Memo)$", ErrorMessage = "Campo inválido")]
        public string tipoCampo { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo orden es requerido")]
        public int orden { get; set; }
        [Required(ErrorMessage = "El campo \"requerido\" es requerido")]
        public bool requerido { get; set; }
        [Required(ErrorMessage = "El campo label es requerido")]
        public string label { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo placeHolder es requerido")]
        public string placeHolder { get; set; } = String.Empty;
        public string opciones { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo \"editable\" es requerido")]
        public bool editable { get; set; }
    }
}