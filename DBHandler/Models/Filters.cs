namespace DBHandler.Models
{
    public class Filters
    {
        public int? Limit { get; set; }

        public string? FechaInicioIngreso { get; set; }

        public string? FechaFinIngreso { get; set; }

        public string? FechaInicioActualizacion { get; set; }

        public string? FechaFinActualizacion { get; set; }

        public int? AsesorId { get; set; }

        public int? FlujoId { get; set; }

        public int? EtapaId { get; set; }
        public int? SubEtapaId { get; set; }

        public string? Estatus { get; set; }

        public string? Asunto { get; set; }

        public int? RemitenteId { get; set; }
        public int? Usuario { get; set; }
    }
}