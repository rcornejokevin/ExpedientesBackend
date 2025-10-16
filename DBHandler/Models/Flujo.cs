using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace DBHandler.Models
{
    [Table("Flujo")]
    [Index(nameof(Activo))]
    [Index(nameof(Nombre), IsUnique = true)]
    public class Flujo
    {
        public Flujo() { }
        public Flujo(string Nombre, string Correlativo, string? Detalle,
         int Activo, int CierreArchivado, int CierreDevolucionAlRemitente,
          int CierreEnviadoAJudicial, int FlujoAsociado)
        {
            this.Nombre = Nombre.Trim();
            this.Correlativo = Correlativo;
            this.Detalle = Detalle ?? "".Trim();
            this.Activo = Activo;
            this.CierreArchivado = CierreArchivado;
            this.CierreDevolucionAlRemitente = CierreDevolucionAlRemitente;
            this.CierreEnviadoAJudicial = CierreEnviadoAJudicial;
            this.FlujoAsociado = FlujoAsociado;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = String.Empty;
        [Required]
        public string Correlativo { get; set; } = String.Empty;
        public string? Detalle { get; set; }
        [Required]
        public int Activo { get; set; } = 1;
        [Required]
        public int CierreArchivado { get; set; } = 1;
        [Required]
        public int CierreDevolucionAlRemitente { get; set; } = 1;
        [Required]
        public int CierreEnviadoAJudicial { get; set; } = 1;
        [Required]
        public int FlujoAsociado { get; set; } = 1;
        [JsonIgnore]
        public virtual ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();
        [JsonIgnore]
        public virtual ICollection<Campo> Campos { get; set; } = new List<Campo>();
    }
}