namespace PIM.Web.Models
{
    /// <summary>
    /// DTO de resposta da API de autenticação
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioDto Usuario { get; set; } = new UsuarioDto();
    }

    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool PrimeiroAcesso { get; set; }
        public bool Ativo { get; set; }
        public int IdSetor { get; set; }
        public int IdPerfil { get; set; }
    }
}
