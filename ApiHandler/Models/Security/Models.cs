using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Security
{
    public class LoginRequest
    {
        [Required]
        public string username { get; set; } = String.Empty;
        [Required]
        public string password { get; set; } = String.Empty;
    }
    public class NewUsuarioRequest
    {
        [Required]
        public string username { get; set; } = String.Empty;
        [Required]
        [RegularExpression("^(Administrador|moderator|user)$", ErrorMessage = "Perfil inválido")]
        public string perfil { get; set; } = String.Empty;
    }
    public class EditUsuarioRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id requerido")]
        public int id { get; set; }
        [Required]
        public string username { get; set; } = String.Empty;
        [Required]
        [RegularExpression("^(Administrador|moderator|user)$", ErrorMessage = "Perfil inválido")]
        public string perfil { get; set; } = String.Empty;
    }
}