using System.ComponentModel.DataAnnotations;

namespace PIM.Api.DTOs.Requests
{
    /// <summary>
    /// Request para análise preliminar de problema pela IA (antes de criar chamado)
    /// </summary>
    public class AnalisePreliminarRequest
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "O título deve ter entre 5 e 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 2000 caracteres")]
        public string Descricao { get; set; } = string.Empty;
    }
}
