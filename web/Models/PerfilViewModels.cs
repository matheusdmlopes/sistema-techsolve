using System.ComponentModel.DataAnnotations;

namespace PIM.Web.Models
{
    /// <summary>
    /// ViewModel para exibir dados completos do usuário
    /// </summary>
    public class UsuarioDetalhadoDto
    {
        public int IdUsuario { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public bool Ativo { get; set; }

        public SetorDto Setor { get; set; } = new();
        public PerfilAcessoDto Perfil { get; set; } = new();
    }

    public class PerfilAcessoDto
    {
        public int IdPerfil { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    /// <summary>
    /// ViewModel para atualizar dados do perfil
    /// </summary>
    public class AtualizarPerfilViewModel
    {
        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string NomeCompleto { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string? Telefone { get; set; }
    }

    /// <summary>
    /// ViewModel para alteração de senha
    /// </summary>
    public class AlterarSenhaViewModel
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
        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}
