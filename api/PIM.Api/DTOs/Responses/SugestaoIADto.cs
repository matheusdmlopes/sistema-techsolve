namespace PIM.Api.DTOs.Responses
{
    public class SugestaoIADto
    {
        public int IdSugestao { get; set; }
        public string TextoSugestao { get; set; } = string.Empty;
        public DateTime DataSugestao { get; set; }
        public bool FoiAceita { get; set; }
    }
}
