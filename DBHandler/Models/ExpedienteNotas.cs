using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace DBHandler.Models
{
    [Table("ExpedienteNotas")]
    [Index(nameof(ExpedienteId))]
    [Index(nameof(FechaIngreso))]
    public class ExpedienteNotas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Expediente")]
        public int ExpedienteId { get; set; }
        [Required]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        [ForeignKey("Usuario")]
        public int? AsesorId { get; set; }
        [JsonIgnore]
        public virtual Usuario Asesor { get; set; } = null!;
        [Column(TypeName = "CLOB")]
        public string? Nota { get; set; }

    }
}