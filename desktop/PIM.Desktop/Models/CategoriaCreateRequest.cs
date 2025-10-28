namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para criação de nova Categoria
    /// </summary>
    public class CategoriaCreateRequest
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}
