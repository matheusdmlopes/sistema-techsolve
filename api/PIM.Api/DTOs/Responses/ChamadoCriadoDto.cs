namespace PIM.Api.DTOs.Responses
{
    /// <summary>
    /// DTO retornado após criação de um chamado
    /// </summary>
    public class ChamadoCriadoDto
    {
        public int IdChamado { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
    }
}
