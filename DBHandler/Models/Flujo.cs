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
        public Flujo(int Id, string Nombre, string Correlativo, string? Detalle,
         bool Activo, bool CierreArchivado, bool CierreDevolucionAlRemitente,
          bool CierreEnviadoAJudicial, bool FlujoAsociado)
        {
            this.Id = Id;
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
        public bool Activo { get; set; } = true;
        [Required]
        public bool CierreArchivado { get; set; } = true;
        [Required]
        public bool CierreDevolucionAlRemitente { get; set; } = true;
        [Required]
        public bool CierreEnviadoAJudicial { get; set; } = true;
        [Required]
        public bool FlujoAsociado { get; set; } = true;
        [JsonIgnore]
        public virtual ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();
        [JsonIgnore]
        public virtual ICollection<Campo> Campos { get; set; } = new List<Campo>();
    }
}