using System.ComponentModel.DataAnnotations;
using DBHandler.Context;

namespace ApiHandler.Models.Validations
{
    public class UniqueNombreAttribute : ValidationAttribute
    {
        private readonly string? _idPropertyName;

        public UniqueNombreAttribute(string? idPropertyName = null)
        {
            _idPropertyName = idPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var nombre = value as string;
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return ValidationResult.Success;
            }

            var db = (DBHandlerContext)validationContext.GetService(typeof(DBHandlerContext))!;

            int currentId = 0;
            if (!string.IsNullOrEmpty(_idPropertyName))
            {
                var idProp = validationContext.ObjectType.GetProperty(_idPropertyName);
                if (idProp != null)
                {
                    var rawId = idProp.GetValue(validationContext.ObjectInstance);
                    if (rawId != null && int.TryParse(rawId.ToString(), out var parsed))
                    {
                        currentId = parsed;
                    }
                }
            }
            var existe = db.Flujos.Any(f => f.Nombre == nombre);
            if (currentId > 0)
            {
                existe = db.Flujos.Any(f => f.Nombre == nombre && f.Id != currentId);
            }
            if (existe)
            {
                return new ValidationResult(ErrorMessage ?? "El nombre ya existe, debe ser único.");
            }

            return ValidationResult.Success;
        }
    }
}
