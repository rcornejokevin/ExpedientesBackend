using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace DBHandler.Models
{
    [Table("Expediente")]
    [Index(nameof(Codigo), IsUnique = true)]
    [Index(nameof(Estatus))]
    [Index(nameof(FechaIngreso))]
    [Index(nameof(Activo))]
    public class Expediente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Codigo { get; set; } = String.Empty;
        [Required]
        public string Nombre { get; set; } = String.Empty;
        [Required]
        public string Asunto { get; set; } = String.Empty;
        [Required]
        public string Estatus { get; set; } = String.Empty;
        [Required]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }

        [Required]
        public bool Activo { get; set; }
        public string? Ubicacion { get; set; }
        [Column(TypeName = "CLOB")]
        public string? CampoValorJson { get; set; }
        public string? NombreArchivo { get; set; }
        public string? NombreArchivoHash { get; set; }
        [ForeignKey("Etapa")]
        [Required]
        public int EtapaId { get; set; }
        [ForeignKey("EtapaDetalle")]
        public int? EtapaDetalleId { get; set; }
        [ForeignKey("Usuario")]
        public int AsesorId { get; set; }
        [ForeignKey("Expediente")]
        public int? ExpedienteRelacionadoId { get; set; }
        [ForeignKey("Remitente")]
        public int RemitenteId { get; set; }
        [JsonIgnore]
        public virtual Etapa Etapa { get; set; } = null!;
        [JsonIgnore]
        public virtual EtapaDetalle EtapaDetalle { get; set; } = null!;
        [JsonIgnore]
        public virtual Usuario Usuario { get; set; } = null!;
        [JsonIgnore]
        public virtual Remitente Remitente { get; set; } = null!;
        [JsonIgnore]
        public virtual Expediente ExpedienteRelacionado { get; set; } = null!;

    }
}