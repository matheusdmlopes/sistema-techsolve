namespace PIM.Web.Models
{
    /// <summary>
    /// DTO para representar um chamado recebido da API
    /// </summary>
    public class ChamadoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string? SolucaoFinal { get; set; }

        public CategoriaDto Categoria { get; set; } = new();
        public PrioridadeDto Prioridade { get; set; } = new();
        public StatusDto Status { get; set; } = new();
        public UsuarioSimplificadoDto UsuarioCriador { get; set; } = new();
        public UsuarioSimplificadoDto? UsuarioTecnico { get; set; }
    }

    /// <summary>
    /// DTO detalhado com relacionamentos completos
    /// </summary>
    public class ChamadoDetalhadoDto
    {
        public int IdChamado { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string? SolucaoFinal { get; set; }

        public CategoriaDto Categoria { get; set; } = new();
        public PrioridadeDto Prioridade { get; set; } = new();
        public StatusDto Status { get; set; } = new();
        public UsuarioComSetorDto UsuarioCriador { get; set; } = new();
        public UsuarioSimplificadoDto? UsuarioTecnico { get; set; }

        public List<HistoricoChamadoDto> Historicos { get; set; } = new();
        public List<AnexoDto> Anexos { get; set; } = new();
        public List<SugestaoIADto> Sugestoes { get; set; } = new();
        public FeedbackDto? Feedback { get; set; }
    }

    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class PrioridadeDto
    {
        public int Id { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class StatusDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class UsuarioSimplificadoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class UsuarioComSetorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public SetorDto? Setor { get; set; }
    }

    public class SetorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class HistoricoChamadoDto
    {
        public int Id { get; set; }
        public DateTime DataMudanca { get; set; }
        public string StatusAnterior { get; set; } = string.Empty;
        public string StatusNovo { get; set; } = string.Empty;
        public string? Observacao { get; set; }
        public UsuarioSimplificadoDto Tecnico { get; set; } = new();
    }

    public class AnexoDto
    {
        public int Id { get; set; }
        public string NomeArquivo { get; set; } = string.Empty;
        public string CaminhoArquivo { get; set; } = string.Empty;
        public DateTime DataUpload { get; set; }
    }

    public class SugestaoIADto
    {
        public int Id { get; set; }
        public string TextoSugestao { get; set; } = string.Empty;
        public DateTime DataSugestao { get; set; }
    }

    public class FeedbackDto
    {
        public int Id { get; set; }
        public int Nota { get; set; }
        public string? Comentario { get; set; }
        public DateTime DataFeedback { get; set; }
    }
}
