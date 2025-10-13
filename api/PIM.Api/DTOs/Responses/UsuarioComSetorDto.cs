namespace PIM.Api.DTOs.Responses
{
    public class UsuarioComSetorDto
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public SetorDto? Setor { get; set; }
    }
}
