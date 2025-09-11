using System.ComponentModel.DataAnnotations;

namespace PIM.Api.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    
    [Required(ErrorMessage = "Nome completo é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome completo deve ter no máximo 100 caracteres")]
    public string NomeCompleto { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 255 caracteres")]
    public string Senha { get; set; } = string.Empty;
    
    [Phone(ErrorMessage = "Telefone deve ter formato válido")]
    [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
    public string? Telefone { get; set; }
    
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public DateTime? UltimoLogin { get; set; }
    public bool PrimeiroAcesso { get; set; } = true;
    public bool Ativo { get; set; } = true;
    
    // Foreign Keys
    [Required(ErrorMessage = "Setor é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdSetor deve ser um valor válido")]
    public int IdSetor { get; set; }
    
    [Required(ErrorMessage = "Perfil de acesso é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdPerfil deve ser um valor válido")]
    public int IdPerfil { get; set; }
    
    // Navigation Properties  
    public Setor Setor { get; set; } = null!;
    public PerfilAcesso Perfil { get; set; } = null!;
    public ICollection<Chamado> ChamadosCriados { get; set; } = [];
    public ICollection<Chamado> ChamadosAtendidos { get; set; } = [];
    public ICollection<HistoricoChamado> HistoricosChamado { get; set; } = [];
}