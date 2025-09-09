namespace BusinessLogic.Models
{
    public class FollowCase
    {

        public int etapaId { get; set; }
        public int subEtapaId { get; set; } = 0;
        public Boolean adjuntarArchivo { get; set; }
        public string nombreArchivo { get; set; } = String.Empty;
        public string archivo { get; set; } = String.Empty;
        public int asesor { get; set; }
    }
}