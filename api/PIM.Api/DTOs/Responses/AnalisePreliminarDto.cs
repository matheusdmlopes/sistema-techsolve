namespace PIM.Api.DTOs.Responses
{
    /// <summary>
    /// DTO de resposta contendo análise preliminar de problema realizada pela IA
    /// </summary>
    /// <remarks>
    /// Retornado pelo endpoint POST /api/ia/analisar-preliminar.
    /// Contém sugestões da IA (GPT-3.5-turbo) para categorização, priorização e resolução do problema.
    /// O usuário pode aceitar as sugestões ou modificá-las antes de criar o chamado.
    /// </remarks>
    public class AnalisePreliminarDto
    {
        /// <summary>
        /// Sugestão de solução gerada pela IA (instruções passo a passo ou explicação do que precisa ser feito)
        /// </summary>
        public string SugestaoSolucao { get; set; } = string.Empty;

        /// <summary>
        /// Categoria sugerida pela IA baseada nas categorias cadastradas no sistema
        /// </summary>
        public CategoriaDto CategoriaSugerida { get; set; } = new();

        /// <summary>
        /// Prioridade sugerida pela IA baseada nas prioridades cadastradas no sistema
        /// </summary>
        public PrioridadeDto PrioridadeSugerida { get; set; } = new();

        /// <summary>
        /// Nível de confiança da análise (valores possíveis: "Alta", "Média", "Baixa")
        /// </summary>
        public string NivelConfianca { get; set; } = "Média";

        /// <summary>
        /// Indica se a IA acredita que o usuário consegue resolver o problema sozinho sem necessidade de técnico
        /// </summary>
        /// <remarks>
        /// Se true, a sugestão de solução contém instruções para o próprio usuário resolver.
        /// Se false, indica que é necessário abrir um chamado para atendimento técnico.
        /// </remarks>
        public bool PodeResolverSemTecnico { get; set; } = false;
    }
}
