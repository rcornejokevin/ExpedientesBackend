using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int Orden { get; set; } = 0;
        [Required]
        public string Tipo { get; set; } = String.Empty;
        [Required]
        public bool Activo { get; set; } = true;
        [Required]
        public bool Requerido { get; set; } = true;
        [ForeignKey("Etapa")]
        public int? EtapaId { get; set; }
        public virtual Etapa Etapa { get; set; } = null!;
        [ForeignKey("EtapaDetalle")]
        public int? EtapaDetalleId { get; set; }
        public virtual EtapaDetalle EtapaDetalle { get; set; } = null!;
    }
}