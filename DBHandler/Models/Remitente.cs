using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DBHandler.Models
{
    [Table("Remitente")]
    [Index(nameof(Activo))]
    public class Remitente
    {
        public Remitente() { }
        public Remitente(int Id, string Descripcion, bool Activo)
        {
            this.Id = Id;
            this.Descripcion = Descripcion;
            this.Activo = Activo;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}