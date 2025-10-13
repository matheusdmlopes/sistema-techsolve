using System.ComponentModel.DataAnnotations;

namespace PIM.Api.DTOs.Requests
{
    public class CriarChamadoRequest
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "A prioridade é obrigatória")]
        public int IdPrioridade { get; set; }

        public int? IdUsuarioCriador { get; set; }
    }
}
