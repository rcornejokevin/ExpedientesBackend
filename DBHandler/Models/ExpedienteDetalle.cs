using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace DBHandler.Models
{
    [Table("ExpedienteDetalle")]
    public class ExpedienteDetalle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Ubicacion { get; set; }
        public string? NombreArchivo { get; set; }
        public string? EstatusAnterior { get; set; } = String.Empty;
        public string? EstatusNuevo { get; set; } = String.Empty;
        public string? NombreArchivoHash { get; set; }
        [ForeignKey("Etapa")]
        public int? EtapaAnteriorId { get; set; }
        [ForeignKey("EtapaDetalle")]
        public int? EtapaDetalleAnteriorId { get; set; }
        [ForeignKey("Etapa")]
        [Required]
        public int EtapaNuevaId { get; set; }
        [ForeignKey("EtapaDetalle")]
        public int? EtapaDetalleNuevaId { get; set; }
        [ForeignKey("Usuario")]
        public int? AsesorAnteriorId { get; set; }
        [Required]
        [ForeignKey("Usuario")]
        public int AsesorNuevorId { get; set; }
        [ForeignKey("Expediente")]
        [Required]
        public int ExpedienteId { get; set; }
        [JsonIgnore]
        public virtual Expediente Expediente { get; set; } = null!;
        [ForeignKey(nameof(EtapaAnteriorId))]
        [JsonIgnore]
        public virtual Etapa? EtapaAnterior { get; set; }

        [ForeignKey(nameof(EtapaNuevaId))]
        [JsonIgnore]
        public virtual Etapa EtapaNueva { get; set; } = null!;

        [ForeignKey(nameof(EtapaDetalleAnteriorId))]
        [JsonIgnore]
        public virtual EtapaDetalle? EtapaDetalleAnterior { get; set; }

        [ForeignKey(nameof(EtapaDetalleNuevaId))]
        [JsonIgnore]
        public virtual EtapaDetalle? EtapaDetalleNueva { get; set; }
    }
}