namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para atualização de Categoria existente
    /// </summary>
    public class CategoriaUpdateRequest
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}
