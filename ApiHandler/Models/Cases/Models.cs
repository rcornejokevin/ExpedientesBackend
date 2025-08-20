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
        public int etapaId { get; set; }
        public int subEtapaId { get; set; }
        public string ubicacion { get; set; } = String.Empty;
        public Campo[] campos { get; set; } = Array.Empty<Campo>();
    }
}