using System.ComponentModel.DataAnnotations;
namespace ApiHandler.Models.Validations
{
    public class UnoOElOtroAttribute : ValidationAttribute
    {
        public string OtroCampo { get; set; } = string.Empty;

        public override bool IsValid(object? value)
        {
            var propiedad = value as string;
            var otraPropiedad = (string?)((object)this).GetType().GetProperty(OtroCampo)?.GetValue(this);

            return (string.IsNullOrEmpty(propiedad) && !string.IsNullOrEmpty(otraPropiedad)) ||
                   (!string.IsNullOrEmpty(propiedad) && string.IsNullOrEmpty(otraPropiedad));
        }
    }
}
