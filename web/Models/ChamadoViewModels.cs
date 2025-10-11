using System.ComponentModel.DataAnnotations;

namespace PIM.Web.Models
{
    /// <summary>
    /// ViewModel para criação de novo chamado
    /// </summary>
    public class NovoChamadoViewModel
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 2000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione uma categoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "Selecione uma prioridade")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma prioridade válida")]
        public int IdPrioridade { get; set; }

        // Listas para popular os dropdowns
        public List<CategoriaDto> Categorias { get; set; } = new();
        public List<PrioridadeDto> Prioridades { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para alterar status do chamado (Técnico)
    /// </summary>
    public class AlterarStatusViewModel
    {
        public int IdChamado { get; set; }
        public string TituloAtual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um status")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um status válido")]
        public int IdStatus { get; set; }

        public string? Observacao { get; set; }

        public string? SolucaoFinal { get; set; }

        public List<StatusDto> StatusDisponiveis { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para dar feedback no chamado
    /// </summary>
    public class FeedbackViewModel
    {
        public int IdChamado { get; set; }
        public string TituloChamado { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione uma nota")]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5")]
        public int Nota { get; set; }

        [StringLength(500, ErrorMessage = "O comentário deve ter no máximo 500 caracteres")]
        public string? Comentario { get; set; }
    }

    /// <summary>
    /// ViewModel para assumir um chamado (Técnico)
    /// </summary>
    public class AssumirChamadoRequest
    {
        public int IdChamado { get; set; }
        public int IdTecnico { get; set; }
    }

    /// <summary>
    /// ViewModel para filtros de chamados
    /// </summary>
    public class FiltrosChamadosViewModel
    {
        public int? IdStatus { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdPrioridade { get; set; }
        public bool? ApenasMeusChamados { get; set; }
        public string? Busca { get; set; }
    }
}
