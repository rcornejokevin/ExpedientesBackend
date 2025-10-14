using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DBHandler.Models
{
    [Table("Usuario")]
    [Index(nameof(Activo))]
    [Index(nameof(Username))]
    public class Usuario
    {
        public Usuario() { }
        public Usuario(int Id, string Username, string Perfil, bool Activo)
        {
            this.Id = Id;
            this.Username = Username;
            this.Perfil = Perfil;
            this.Activo = Activo;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Perfil { get; set; } = null!;
        [Required]
        [Column(TypeName = "NUMBER(1)")]
        public bool Operativo { get; set; } = true;
        [Required]
        [Column(TypeName = "NUMBER(1)")]
        public bool Activo { get; set; } = true;
    }
}