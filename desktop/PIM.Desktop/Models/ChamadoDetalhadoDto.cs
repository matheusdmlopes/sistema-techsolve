using System;
using System.Collections.Generic;

namespace PIM.Desktop.Models
{
    // DTO para receber detalhes completos do chamado da API
    public class ChamadoDetalhadoDto
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

        public List<HistoricoInfo> Historicos { get; set; }

        // Classes aninhadas
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

        public class HistoricoInfo
        {
            public int IdHistorico { get; set; }
            public DateTime DataMudanca { get; set; }
            public string StatusAnterior { get; set; }
            public string StatusNovo { get; set; }
            public string Observacao { get; set; }
            public UsuarioInfo Tecnico { get; set; }
        }
    }
}
