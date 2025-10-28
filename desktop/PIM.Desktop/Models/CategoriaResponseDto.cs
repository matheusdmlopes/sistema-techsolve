namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para resposta da API de Categorias (GET /api/Categorias)
    /// </summary>
    public class CategoriaResponseDto
    {
        public int IdCategoria { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}
