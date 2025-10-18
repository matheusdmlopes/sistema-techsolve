namespace PIM.Api.DTOs.Responses
{
    /// <summary>
    /// DTO com informações de um comentário/mensagem de um chamado
    /// </summary>
    public class ComentarioDto
    {
        public int IdComentario { get; set; }
        public int IdChamado { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataHoraEnvio { get; set; }
        public bool Lido { get; set; }
        public bool EhTecnico { get; set; } // Para diferenciar visualmente no frontend
    }
}
