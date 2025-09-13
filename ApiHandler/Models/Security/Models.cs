using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Security
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string username { get; set; } = String.Empty;
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string password { get; set; } = String.Empty;
    }
    public class NewUsuarioRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string username { get; set; } = String.Empty;
        [Required(ErrorMessage = "El perfil es requerido")]
        [RegularExpression("^(Administrador|Usuario)$", ErrorMessage = "Perfil inválido")]
        public string perfil { get; set; } = String.Empty;
    }
    public class EditUsuarioRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id requerido")]
        public int id { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string username { get; set; } = String.Empty;
        [Required(ErrorMessage = "El perfil es requerido")]
        [RegularExpression("^(Administrador|Usuario)$", ErrorMessage = "Perfil inválido")]
        public string perfil { get; set; } = String.Empty;
    }
}
