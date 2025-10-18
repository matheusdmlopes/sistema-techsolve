using System.ComponentModel.DataAnnotations;

namespace PIM.Api.DTOs.Requests
{
    /// <summary>
    /// Request para enviar um comentário/mensagem em um chamado
    /// </summary>
    public class EnviarComentarioRequest
    {
        [Required(ErrorMessage = "A mensagem é obrigatória")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "A mensagem deve ter entre 1 e 2000 caracteres")]
        public string Mensagem { get; set; } = string.Empty;
    }
}
