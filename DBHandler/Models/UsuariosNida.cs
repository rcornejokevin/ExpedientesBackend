using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBHandler.Models
{
    [Table("USUARIOSNIDA")]
    public class UsuariosNida
    {
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }

        [Column("NOMBREUSUARIO")]
        public string? NombreUsuario { get; set; }

        [Column("IDROL")]
        public int? IdRol { get; set; }

        [Column("IDSISTEMA")]
        public int? IdSistema { get; set; }

        [Column("NIT")]
        public string? Nit { get; set; }

        [Column("CLAVE")]
        public string? Clave { get; set; }

        [Column("NOMBRE")]
        public string? Nombre { get; set; }

        [Column("FECHACREACION")]
        public DateTime? FechaCreacion { get; set; }

        [Column("CAMBIARCLAVE")]
        public int? CambiarClave { get; set; }

        [Column("BLOQUEADO")]
        public int? Bloqueado { get; set; }

        [Column("FIRMA")]
        public string? Firma { get; set; }

        [Column("ULTIMO_CAMBIO_CLAVE")]
        public DateTime? UltimoCambioClave { get; set; }

        [Column("FIRMA_ELECTRONICA")]
        public string? FirmaElectronica { get; set; }

        [Column("DIRECTOR_GESTION")]
        public string? DirectorGestion { get; set; }

        [Column("USERNAME_FEA")]
        public string? UsernameFea { get; set; }

        [Column("PASSWORD_FEA")]
        public string? PasswordFea { get; set; }

        [Column("CLAVE_FEA")]
        public string? ClaveFea { get; set; }

        [Column("PIN_FEA")]
        public string? PinFea { get; set; }

        [Column("VERSION_FEA")]
        public string? VersionFea { get; set; }

        [Column("USUARIO_FEA")]
        public string? UsuarioFea { get; set; }

        [Column("CODIGO_VERIFICACION")]
        public string? CodigoVerificacion { get; set; }

        [Column("ESTADO_CODIGO")]
        public string? EstadoCodigo { get; set; }
    }
}