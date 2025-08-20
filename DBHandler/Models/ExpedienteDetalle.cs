using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("Expediente")]
        [Required]
        public int ExpedienteId { get; set; }
        [Required]
        public string Responsable { get; set; } = String.Empty;
        public virtual Expediente Expediente { get; set; } = null!;
    }
}