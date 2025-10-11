using System.ComponentModel.DataAnnotations;

namespace PIM.Web.Models
{
    /// <summary>
    /// ViewModel para troca de senha no primeiro acesso
    /// </summary>
    public class PrimeiroAcessoViewModel
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        [DataType(DataType.Password)]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [DataType(DataType.Password)]
        [Compare("NovaSenha", ErrorMessage = "A nova senha e a confirmação não coincidem")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;

        public string NomeUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
