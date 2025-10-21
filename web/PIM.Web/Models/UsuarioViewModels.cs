using System.ComponentModel.DataAnnotations;

namespace PIM.Web.Models
{
    /// <summary>
    /// DTO para listagem de usuários
    /// </summary>
    public class UsuarioListaDto
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public bool Ativo { get; set; }
        public SetorSimplificadoDto Setor { get; set; } = new();
        public PerfilSimplificadoDto Perfil { get; set; } = new();
    }

    public class SetorSimplificadoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class PerfilSimplificadoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para criar novo usuário
    /// </summary>
    public class CriarUsuarioViewModel
    {
        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Selecione um setor")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um setor válido")]
        public int IdSetor { get; set; }

        [Required(ErrorMessage = "Selecione um perfil")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um perfil válido")]
        public int IdPerfil { get; set; }

        // Listas para dropdowns
        public List<SetorDto> Setores { get; set; } = new();
        public List<PerfilAcessoDto> Perfis { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para editar usuário existente
    /// </summary>
    public class EditarUsuarioViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Display(Name = "Email (não pode ser alterado)")]
        public string Email { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Selecione um setor")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um setor válido")]
        public int IdSetor { get; set; }

        [Required(ErrorMessage = "Selecione um perfil")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um perfil válido")]
        public int IdPerfil { get; set; }

        public bool Ativo { get; set; }

        // Listas para dropdowns
        public List<SetorDto> Setores { get; set; } = new();
        public List<PerfilAcessoDto> Perfis { get; set; } = new();
    }
}
