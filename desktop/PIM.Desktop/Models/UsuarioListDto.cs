namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para resposta da API de usuários (GET /api/Usuarios)
    /// </summary>
    public class UsuarioApiResponse
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public SetorInfo Setor { get; set; }
        public PerfilInfo Perfil { get; set; }
        public bool Ativo { get; set; }

        public class SetorInfo
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }

        public class PerfilInfo
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }
    }

    /// <summary>
    /// DTO simplificado para lista de usuários (uso interno)
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
