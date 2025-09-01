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
        public string Estatus { get; set; } = String.Empty;
        [Required]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
        [ForeignKey("Etapa")]
        [Required]
        public int EtapaId { get; set; }
        [JsonIgnore]
        public virtual Etapa Etapa { get; set; } = null!;
        [ForeignKey("EtapaDetalle")]
        public int? EtapaDetalleId { get; set; }
        [JsonIgnore]
        public virtual EtapaDetalle EtapaDetalle { get; set; } = null!;
        [Required]
        public bool Activo { get; set; }
        [Required]
        public string Ubicacion { get; set; } = String.Empty;
    }
}