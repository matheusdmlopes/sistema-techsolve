namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO simplificado para lista de usuários
    /// </summary>
    public class UsuarioListDto
    {
        public int IdUsuario { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Setor { get; set; }
        public string PerfilAcesso { get; set; }
        public bool Ativo { get; set; }
    }
}
