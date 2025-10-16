using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace DBHandler.Models
{
    [Table("Etapa_Detalle")]
    [Index(nameof(Activo))]
    public class EtapaDetalle
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public int Orden { get; set; } = 0;
        [Required]
        public int Activo { get; set; } = 1;
        public string? Detalle { get; set; } = String.Empty;
        [ForeignKey("Etapa")]
        [Required]
        public int EtapaId { get; set; }
        [JsonIgnore]
        public virtual Etapa Etapa { get; set; } = null!;
    }

}