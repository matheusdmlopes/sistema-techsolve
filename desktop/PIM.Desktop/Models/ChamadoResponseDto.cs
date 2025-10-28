using System;

namespace PIM.Desktop.Models
{
    // Classe para receber a resposta da API (com objetos aninhados)
    public class ChamadoResponseDto
    {
        public int IdChamado { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string SolucaoFinal { get; set; }

        public CategoriaInfo Categoria { get; set; }
        public PrioridadeInfo Prioridade { get; set; }
        public StatusInfo Status { get; set; }
        public UsuarioInfo UsuarioCriador { get; set; }
        public UsuarioInfo UsuarioTecnico { get; set; }

        // Classes aninhadas para corresponder à estrutura da API
        public class CategoriaInfo
        {
            public int IdCategoria { get; set; }
            public string Nome { get; set; }
            public string Descricao { get; set; }
        }

        public class PrioridadeInfo
        {
            public int IdPrioridade { get; set; }
            public string Nivel { get; set; }
            public string Descricao { get; set; }
        }

        public class StatusInfo
        {
            public int IdStatus { get; set; }
            public string Nome { get; set; }
            public string Descricao { get; set; }
        }

        public class UsuarioInfo
        {
            public int IdUsuario { get; set; }
            public string Nome { get; set; }
        }
    }
}
