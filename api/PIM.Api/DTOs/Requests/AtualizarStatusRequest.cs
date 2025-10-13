using System.ComponentModel.DataAnnotations;

namespace PIM.Api.DTOs.Requests
{
    public class AtualizarStatusRequest
    {
        [Required(ErrorMessage = "O status é obrigatório")]
        public int IdStatus { get; set; }

        [StringLength(1000, ErrorMessage = "A observação deve ter no máximo 1000 caracteres")]
        public string? Observacao { get; set; }

        [StringLength(2000, ErrorMessage = "A solução final deve ter no máximo 2000 caracteres")]
        public string? SolucaoFinal { get; set; }

        public int? IdUsuarioTecnico { get; set; }
    }
}
