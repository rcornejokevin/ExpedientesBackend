using System.ComponentModel.DataAnnotations;
using ApiHandler.Models.Validations;

namespace ApiHandler.Models.Catalog
{
    public class NewFlujoRequest
    {
        [Required(ErrorMessage = "El nombre del flujo es requerido")]
        [UniqueNombre(ErrorMessage = "El nombre debe ser único.")]
        public string nombre { get; set; } = String.Empty;
        [Required(ErrorMessage = "El correlativo del flujo es requerido")]
        [UniqueCorrelativo(ErrorMessage = "El correlativo debe ser único.")]
        public string correlativo { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo 'Cierre Archivado' es obligatorio")]
        public bool cierreArchivado { get; set; }
        [Required(ErrorMessage = "El campo 'Cierre Devolucion Al Remitente' es obligatorio")]
        public bool cierreDevolucionAlRemitente { get; set; }
        [Required(ErrorMessage = "El campo 'Cierre Enviado a Judicial' es obligatorio")]
        public bool cierreEnviadoAJudicial { get; set; }
        [Required(ErrorMessage = "El campo 'Flujo Asociado' es obligatorio")]
        public bool flujoAsociado { get; set; }

    }
    public class EditFlujoRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "El Id es obligatorio")]
        public int id { get; set; }
        [Required(ErrorMessage = "El nombre del flujo es requerido")]
        [UniqueNombre(nameof(id), ErrorMessage = "El nombre debe ser único.")]
        public string nombre { get; set; } = String.Empty;
        [Required(ErrorMessage = "El correlativo del flujo es requerido")]
        [UniqueCorrelativo(nameof(id), ErrorMessage = "El correlativo debe ser único.")]
        public string correlativo { get; set; } = String.Empty;
        public string detalle { get; set; } = String.Empty;
        [Required(ErrorMessage = "El campo 'Cierre Archivado' es obligatorio")]
        public bool cierreArchivado { get; set; }
        [Required(ErrorMessage = "El campo 'Cierre Devolucion Al Remitente' es obligatorio")]
        public bool cierreDevolucionAlRemitente { get; set; }
        [Required(ErrorMessage = "El campo 'Cierre Enviado a Judicial' es obligatorio")]
        public bool cierreEnviadoAJudicial { get; set; }
        [Required(ErrorMessage = "El campo 'Flujo Asociado' es obligatorio")]
        public bool flujoAsociado { get; set; }
    }
}