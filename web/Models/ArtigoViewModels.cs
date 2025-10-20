namespace PIM.Web.Models
{
    /// <summary>
    /// Model para artigos de problemas comuns
    /// </summary>
    public class ArtigoViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Resumo { get; set; } = string.Empty;
        public string Problema { get; set; } = string.Empty;
        public string Solucao { get; set; } = string.Empty;
        public string IconeCategoria { get; set; } = string.Empty;
    }
}
