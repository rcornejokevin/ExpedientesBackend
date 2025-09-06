using System.ComponentModel.DataAnnotations;
using DBHandler.Models;

namespace ApiHandler.Models.Cases
{
    public class NewCasesRequest
    {
        [Required]
        public string codigo { get; set; } = String.Empty;
        [Required]
        public string nombre { get; set; } = String.Empty;
        [Required]
        public int etapaId { get; set; }
        public int subEtapaId { get; set; }
        public string nombreArchivo { get; set; } = String.Empty;
        public string archivo { get; set; } = String.Empty;
        public DateTime fechaIngreso { get; set; }
        [Required]
        public int asesor { get; set; }
        public string campos { get; set; } = String.Empty;
    }
}