using System;

namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO completo para detalhes/edição de usuário
    /// </summary>
    public class UsuarioDetailDto
    {
        public int IdUsuario { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public int IdSetor { get; set; }
        public int IdPerfil { get; set; }
        public bool Ativo { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? UltimoLogin { get; set; }
    }

    /// <summary>
    /// DTO para criar novo usuário
    /// </summary>
    public class CriarUsuarioRequest
    {
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public int IdSetor { get; set; }
        public int IdPerfil { get; set; }
    }

    /// <summary>
    /// DTO para atualizar usuário
    /// </summary>
    public class AtualizarUsuarioRequest
    {
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public int IdSetor { get; set; }
        public int IdPerfil { get; set; }
        public bool Ativo { get; set; }
    }

    /// <summary>
    /// DTO para setor
    /// </summary>
    public class SetorDto
    {
        public int IdSetor { get; set; }
        public string NomeSetor { get; set; }
    }

    /// <summary>
    /// DTO para perfil de acesso
    /// </summary>
    public class PerfilDto
    {
        public int IdPerfil { get; set; }
        public string NomePerfil { get; set; }
    }
}
