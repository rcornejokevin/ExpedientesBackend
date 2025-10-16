using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace DBHandler.Models
{
    [Table("Campo")]
    [Index(nameof(Activo))]
    public class Campo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string Placeholder { get; set; } = string.Empty;
        [Column(TypeName = "CLOB")]
        public string? Opciones { get; set; }
        [Required]
        public int Orden { get; set; } = 0;
        [Required]
        public string Tipo { get; set; } = String.Empty;
        [Required]
        public int Activo { get; set; } = 1;
        [Required]
        public int Requerido { get; set; } = 1;
        [Required]
        public int Editable { get; set; } = 1;
        [ForeignKey("Flujo")]
        public int? FlujoId { get; set; }
        [JsonIgnore]
        public virtual Flujo Flujo { get; set; } = null!;
        [ForeignKey("Etapa")]
        public int? EtapaId { get; set; }
        [JsonIgnore]
        public virtual Etapa Etapa { get; set; } = null!;

    }
}