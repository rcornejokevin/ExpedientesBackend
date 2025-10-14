using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace DBHandler.Models
{
    [Table("Etapa")]
    [Index(nameof(Activo))]

    public class Etapa
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public int Orden { get; set; } = 0;
        public string? Detalle { get; set; } = String.Empty;
        [Required]
        [Column(TypeName = "NUMBER(1)")]
        public bool Activo { get; set; } = true;
        [Required]
        [Column(TypeName = "NUMBER(1)")]
        public bool FinDeFlujo { get; set; } = false;
        [ForeignKey("Flujo")]
        [Required]
        public int FlujoId { get; set; }
        [JsonIgnore]
        public virtual Flujo Flujo { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<EtapaDetalle> EtapaDetalles { get; set; } = new List<EtapaDetalle>();
    }
}