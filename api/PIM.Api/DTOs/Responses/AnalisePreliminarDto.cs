namespace PIM.Api.DTOs.Responses
{
    /// <summary>
    /// Response com análise preliminar da IA sobre um problema
    /// </summary>
    public class AnalisePreliminarDto
    {
        /// <summary>
        /// Sugestão de solução gerada pela IA
        /// </summary>
        public string SugestaoSolucao { get; set; } = string.Empty;

        /// <summary>
        /// Categoria sugerida pela IA
        /// </summary>
        public CategoriaDto CategoriaSugerida { get; set; } = new();

        /// <summary>
        /// Prioridade sugerida pela IA
        /// </summary>
        public PrioridadeDto PrioridadeSugerida { get; set; } = new();

        /// <summary>
        /// Nível de confiança da análise (Alta, Média, Baixa)
        /// </summary>
        public string NivelConfianca { get; set; } = "Média";

        /// <summary>
        /// Indica se a IA acredita que o problema pode ser resolvido sem técnico
        /// </summary>
        public bool PodeResolverSemTecnico { get; set; } = false;
    }
}
