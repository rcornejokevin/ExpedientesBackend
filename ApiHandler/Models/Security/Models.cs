using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Security
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string username { get; set; } = String.Empty;
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string password { get; set; } = String.Empty;
        [Required(ErrorMessage = "El dispositivo es requerido")]
        public string device { get; set; } = String.Empty;
    }
    public class TwoFactorRequest
    {
        [Required(ErrorMessage = "El token es requerido")]
        public string token { get; set; } = String.Empty;
    }
    public class NewUsuarioRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string username { get; set; } = String.Empty;
        [Required(ErrorMessage = "El email es requerido")]
        public string email { get; set; } = String.Empty;
        [Required(ErrorMessage = "El perfil es requerido")]
        [RegularExpression("^(ADMINISTRADOR|RECEPCIÓN|IT|ASESOR|PROCURADOR)$", ErrorMessage = "Perfil inválido")]
        public string perfil { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo 'operativo' es obligatorio")]
        public bool operativo { get; set; }
    }
    public class EditUsuarioRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id requerido")]
        public int id { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string username { get; set; } = String.Empty;
        [Required(ErrorMessage = "El email es requerido")]
        public string email { get; set; } = String.Empty;
        [Required(ErrorMessage = "El perfil es requerido")]
        [RegularExpression("^(ADMINISTRADOR|RECEPCIÓN|IT|ASESOR|PROCURADOR)$", ErrorMessage = "Perfil inválido")]
        public string perfil { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo 'operativo' es obligatorio")]
        public bool operativo { get; set; }
    }
    public class ChangePasswordRequest
    {
        public int id { get; set; }
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        public string newPassword { get; set; } = String.Empty;
    }
}
