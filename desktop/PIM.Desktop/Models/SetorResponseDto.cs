namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para resposta da API de Setores (GET /api/Setores)
    /// Alinhado com o formato retornado pela API
    /// </summary>
    public class SetorResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}
