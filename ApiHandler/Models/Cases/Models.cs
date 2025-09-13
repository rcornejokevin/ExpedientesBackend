using System.ComponentModel.DataAnnotations;
using DBHandler.Models;

namespace ApiHandler.Models.Cases
{
    public class NewCasesRequest
    {
        [Required(ErrorMessage = "El codigo es requerido")]
        public string codigo { get; set; } = String.Empty;
        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; } = String.Empty;
        [Required(ErrorMessage = "El flujo es requerido")]
        public int flujoId { get; set; }
        public string nombreArchivo { get; set; } = String.Empty;
        public string archivo { get; set; } = String.Empty;
        public DateTime fechaIngreso { get; set; }
        [Required(ErrorMessage = "El asesor es requerido")]
        public int asesor { get; set; }
        [Required(ErrorMessage = "El remitente es requerido")]
        public int remitenteId { get; set; }
        [Required(ErrorMessage = "El asunto es requerido")]
        public string asunto { get; set; } = String.Empty;
        public string campos { get; set; } = String.Empty;
    }
    public class EditCasesRequest
    {
        [Required(ErrorMessage = "El id es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El id es requerido")]
        public int id { get; set; }
        [Required(ErrorMessage = "La etapa es requerida")]
        public int etapaId { get; set; }
        public int subEtapaId { get; set; }
        public bool adjuntarArchivo { get; set; }
        public string nombreArchivo { get; set; } = String.Empty;
        public string archivo { get; set; } = String.Empty;
        [Required(ErrorMessage = "El asesor es requerido")]
        public int asesor { get; set; }
    }
    public class EditCaseStateRequest
    {
        [Required(ErrorMessage = "El id es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El id es requerido")]
        public int id { get; set; }
        [RegularExpression("^(Cerrado|Archivado)$", ErrorMessage = "Estado inválido")]
        [Required(ErrorMessage = "El estado es requerido")]
        public string state { get; set; } = String.Empty;
    }

}