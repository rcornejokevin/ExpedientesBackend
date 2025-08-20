namespace ApiHandler.Models
{
    public class Response
    {
        public string code { get; set; } = String.Empty;
        public string message { get; set; } = String.Empty;
        public object? data { get; set; }
    }
}