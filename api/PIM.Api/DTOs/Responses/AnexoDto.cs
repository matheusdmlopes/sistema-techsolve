namespace PIM.Api.DTOs.Responses
{
    public class AnexoDto
    {
        public int IdAnexo { get; set; }
        public string NomeArquivo { get; set; } = string.Empty;
        public string CaminhoArquivo { get; set; } = string.Empty;
        public DateTime DataUpload { get; set; }
    }
}
