using System.ComponentModel.DataAnnotations;

namespace PIM.Api.DTOs.Requests
{
    public class AtribuirTecnicoRequest
    {
        [Required(ErrorMessage = "O ID do técnico é obrigatório")]
        public int IdUsuarioTecnico { get; set; }
    }
}
