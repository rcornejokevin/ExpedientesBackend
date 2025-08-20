using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace DBHandler.Models
{
    [Table("Flujo")]
    [Index(nameof(Activo))]
    [Index(nameof(Nombre), IsUnique = true)]
    public class Flujo
    {
        public Flujo() { }
        public Flujo(int Id, string Nombre, string? Detalle, bool Activo)
        {
            this.Id = Id;
            this.Nombre = Nombre.Trim();
            this.Detalle = Detalle ?? "".Trim();
            this.Activo = Activo;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = String.Empty;
        public string? Detalle { get; set; }
        [Required]
        public bool Activo { get; set; } = true;
        [JsonIgnore]
        public virtual ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();
    }
}