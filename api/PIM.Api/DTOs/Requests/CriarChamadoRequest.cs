using System.ComponentModel.DataAnnotations;

namespace PIM.Api.DTOs.Requests
{
    /// <summary>
    /// DTO para requisição de criação de novo chamado técnico
    /// </summary>
    /// <remarks>
    /// Usado pelo endpoint POST /api/chamados para criar um novo ticket.
    /// O ID do usuário criador pode ser fornecido ou extraído do token JWT.
    /// Categoria e prioridade podem vir da sugestão da IA ou serem escolhidos manualmente.
    /// </remarks>
    public class CriarChamadoRequest
    {
        /// <summary>
        /// Título resumido do problema (máximo 200 caracteres)
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do problema (máximo 2000 caracteres)
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// ID da categoria do problema (Hardware, Software, Rede, etc.)
        /// </summary>
        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int IdCategoria { get; set; }

        /// <summary>
        /// ID da prioridade do chamado (Baixa, Média, Alta, Crítica)
        /// </summary>
        [Required(ErrorMessage = "A prioridade é obrigatória")]
        public int IdPrioridade { get; set; }

        /// <summary>
        /// ID do usuário criador (opcional - se não fornecido, será extraído do token JWT)
        /// </summary>
        public int? IdUsuarioCriador { get; set; }
    }
}
