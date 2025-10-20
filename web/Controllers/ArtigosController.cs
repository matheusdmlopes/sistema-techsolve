using Microsoft.AspNetCore.Mvc;
using PIM.Web.Models;
using PIM.Web.Filters;

namespace PIM.Web.Controllers
{
    /// <summary>
    /// Controller para exibir artigos de problemas comuns
    /// </summary>
    [AuthorizeSession]
    public class ArtigosController : Controller
    {
        private static readonly List<ArtigoViewModel> Artigos = new()
        {
            new ArtigoViewModel
            {
                Id = 1,
                Titulo = "Computador Não Liga",
                Categoria = "Hardware",
                IconeCategoria = "bi-cpu",
                Resumo = "Soluções para quando o computador não inicializa",
                Problema = "O computador não responde ao pressionar o botão de ligar ou liga mas não exibe imagem na tela.",
                Solucao = "Verifique: 1) Cabo de energia conectado e tomada funcionando; 2) Botão de energia da fonte ligado; 3) Conexões de memória RAM bem encaixadas; 4) Cabo do monitor conectado corretamente. Se o LED da placa-mãe acender, teste remover e recolocar a memória RAM."
            },
            new ArtigoViewModel
            {
                Id = 2,
                Titulo = "Internet Lenta ou Instável",
                Categoria = "Rede",
                IconeCategoria = "bi-wifi",
                Resumo = "Como resolver problemas de conexão lenta",
                Problema = "A conexão com internet está muito lenta, cai constantemente ou não consegue acessar determinados sites.",
                Solucao = "Soluções: 1) Reinicie o roteador (desligue 30 segundos); 2) Teste com cabo de rede ao invés de Wi-Fi; 3) Verifique se outros dispositivos estão consumindo banda; 4) Limpe cache do navegador; 5) Execute ipconfig /flushdns no CMD para limpar DNS; 6) Teste alterar DNS para 8.8.8.8 (Google)."
            },
            new ArtigoViewModel
            {
                Id = 3,
                Titulo = "Sistema Operacional Lento",
                Categoria = "Software",
                IconeCategoria = "bi-speedometer",
                Resumo = "Melhorar desempenho do Windows",
                Problema = "O Windows está lento para iniciar, abrir programas demora muito e o sistema trava constantemente.",
                Solucao = "Ações recomendadas: 1) Desative programas de inicialização desnecessários no Gerenciador de Tarefas; 2) Execute limpeza de disco (cleanmgr); 3) Verifique uso de disco/CPU no Gerenciador de Tarefas; 4) Execute SFC /scannow no CMD como administrador; 5) Considere adicionar mais memória RAM se o uso está sempre alto."
            },
            new ArtigoViewModel
            {
                Id = 4,
                Titulo = "Impressora Não Imprime",
                Categoria = "Hardware",
                IconeCategoria = "bi-printer",
                Resumo = "Resolver problemas comuns de impressão",
                Problema = "A impressora está ligada mas não imprime, aparece offline ou os documentos ficam presos na fila.",
                Solucao = "Tente: 1) Verifique se há papel e tinta/toner; 2) Reinicie a impressora e o computador; 3) Cancele todos os trabalhos na fila de impressão; 4) Verifique conexão USB ou de rede; 5) No Painel de Controle, remova e reinstale a impressora; 6) Atualize o driver no site do fabricante."
            },
            new ArtigoViewModel
            {
                Id = 5,
                Titulo = "Arquivos Deletados Acidentalmente",
                Categoria = "Software",
                IconeCategoria = "bi-file-earmark-x",
                Resumo = "Recuperar arquivos excluídos",
                Problema = "Arquivos importantes foram deletados acidentalmente e não estão mais na Lixeira.",
                Solucao = "Opções de recuperação: 1) Verifique a Lixeira primeiro; 2) No Windows, use Histórico de Arquivos se estiver ativado; 3) Verifique backups em nuvem (OneDrive, Google Drive); 4) Use software de recuperação como Recuva ou TestDisk; 5) IMPORTANTE: Pare de usar o disco imediatamente para evitar sobrescrever dados. Se crítico, contate TI."
            },
            new ArtigoViewModel
            {
                Id = 6,
                Titulo = "Senha de Rede Wi-Fi Esquecida",
                Categoria = "Rede",
                IconeCategoria = "bi-shield-lock",
                Resumo = "Recuperar ou resetar senha Wi-Fi",
                Problema = "Preciso conectar um novo dispositivo mas não lembro a senha da rede Wi-Fi.",
                Solucao = "Como descobrir: 1) No Windows conectado, vá em Configurações > Rede > Status > Central de Rede > clique na rede Wi-Fi > Propriedades sem fio > Segurança > marque 'Mostrar caracteres'; 2) Acesse o painel do roteador (geralmente 192.168.0.1 ou 192.168.1.1) com login admin; 3) Se não conseguir acesso, último recurso é resetar o roteador (botão reset 10 segundos)."
            }
        };

        public IActionResult Index()
        {
            return View(Artigos);
        }

        public IActionResult Detalhes(int id)
        {
            var artigo = Artigos.FirstOrDefault(a => a.Id == id);

            if (artigo == null)
            {
                TempData["Erro"] = "Artigo não encontrado";
                return RedirectToAction("Index");
            }

            return View(artigo);
        }
    }
}
