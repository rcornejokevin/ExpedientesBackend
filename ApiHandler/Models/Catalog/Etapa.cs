using System.ComponentModel.DataAnnotations;

namespace ApiHandler.Models.Catalog
{
    public class NewEtapaRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; } = String.Empty;
        [Required(ErrorMessage = "El orden es requerido")]
        public int orden { get; set; }
        public string detalle { get; set; } = String.Empty;
        [Required(ErrorMessage = "El flujo es requerido")]
        public int flujoId { get; set; }
        [Required(ErrorMessage = "Fin de flujo es requerido")]
        public bool finDeFlujo { get; set; }
    }
    public class EditEtapaRequest
    {
        [Required(ErrorMessage = "El id es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El valor de id es requerido")]
        public int id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; } = String.Empty;
        [Required(ErrorMessage = "El orden es requerido")]
        public int orden { get; set; }
        public string detalle { get; set; } = String.Empty;
        [Required(ErrorMessage = "El flujo es requerido")]
        public int flujoId { get; set; }
        [Required(ErrorMessage = "Fin de flujo es requerido")]
        public bool finDeFlujo { get; set; }
    }
    public class OrdenDetail
    {
        [Required(ErrorMessage = "Se debe enviar el id a ordenar")]
        public int id { get; set; }

        [Required(ErrorMessage = "Se debe enviar el campo orden")]
        public int orden { get; set; }
    }

    public class EditOrdenEtapaRequest
    {
        [Required(ErrorMessage = "Se debe enviar el listado")]
        [MinLength(1, ErrorMessage = "Debe enviar al menos un item.")]
        public List<OrdenDetail> items { get; set; } = new();
    }
}