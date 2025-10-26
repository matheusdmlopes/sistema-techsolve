namespace PIM.Desktop.Models
{
    /// <summary>
    /// Modelo de resposta do login
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; }
        public UsuarioDto Usuario { get; set; }
    }

    /// <summary>
    /// Dados do usuário retornados no login
    /// </summary>
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Setor { get; set; }
        public int IdSetor { get; set; }
        public string Perfil { get; set; }
        public int IdPerfil { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public bool Ativo { get; set; }
    }
}
