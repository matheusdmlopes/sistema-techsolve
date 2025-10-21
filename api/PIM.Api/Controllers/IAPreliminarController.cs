using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using PIM.Api.Data;
using PIM.Api.DTOs.Requests;
using PIM.Api.DTOs.Responses;

namespace PIM.Api.Controllers;

/// <summary>
/// Controller para análise preliminar de problemas utilizando Inteligência Artificial
/// </summary>
/// <remarks>
/// Utiliza OpenAI GPT-3.5-turbo para analisar descrições de problemas antes de criar um chamado.
/// A IA sugere categoria, prioridade, e uma possível solução.
/// Também indica se o problema pode ser resolvido pelo próprio usuário sem necessidade de técnico.
/// </remarks>
[ApiController]
[Route("api/ia")]
[Authorize]
public class IAPreliminarController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IAPreliminarController> _logger;

    public IAPreliminarController(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<IAPreliminarController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Analisa um problema descrito pelo usuário usando IA
    /// </summary>
    /// <remarks>
    /// Envia a descrição do problema para o modelo GPT-3.5-turbo da OpenAI.
    /// A IA retorna:
    /// - Categoria sugerida (baseada nas categorias cadastradas no sistema)
    /// - Prioridade sugerida (baseada nas prioridades cadastradas)
    /// - Sugestão de solução (orientações para o usuário)
    /// - Indicador se pode ser resolvido sem técnico
    ///
    /// A resposta da IA é parseada e validada contra as categorias e prioridades existentes no banco.
    /// Se a IA sugerir categoria/prioridade inexistente, usa fallback padrão.
    /// </remarks>
    /// <param name="request">Título e descrição do problema a ser analisado</param>
    /// <returns>Análise preliminar com sugestões da IA</returns>
    /// <response code="200">Análise realizada com sucesso</response>
    /// <response code="400">Sistema sem categorias ou prioridades cadastradas</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro ao processar análise (falha na API OpenAI ou configuração inválida)</response>
    [HttpPost("analisar-preliminar")]
    public async Task<ActionResult<AnalisePreliminarDto>> AnalisarProblema([FromBody] AnalisePreliminarRequest request)
    {
        try
        {
            // Validar chave da API OpenAI
            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Chave da API OpenAI não configurada");
                return StatusCode(500, new { erro = "Serviço de IA não configurado" });
            }

            // Buscar categorias e prioridades disponíveis
            var categorias = await _context.Categorias.ToListAsync();
            var prioridades = await _context.Prioridades.ToListAsync();

            if (!categorias.Any() || !prioridades.Any())
            {
                return BadRequest(new { erro = "Sistema não possui categorias ou prioridades cadastradas" });
            }

            var listaCategoriasTexto = string.Join(", ", categorias.Select(c => c.NomeCategoria));
            var listaPrioridadesTexto = string.Join(", ", prioridades.Select(p => p.NivelPrioridade));

            // Criar cliente OpenAI
            var client = new ChatClient("gpt-3.5-turbo", apiKey);

            // Prompt otimizado para análise preliminar
            var prompt = $@"Você é um assistente de suporte técnico inteligente. Analise o seguinte problema e forneça uma resposta estruturada.

TÍTULO: {request.Titulo}
DESCRIÇÃO: {request.Descricao}

CATEGORIAS DISPONÍVEIS: {listaCategoriasTexto}
PRIORIDADES DISPONÍVEIS: {listaPrioridadesTexto}

Forneça sua análise no seguinte formato EXATO (uma resposta por linha):

CATEGORIA: [escolha UMA categoria da lista acima]
PRIORIDADE: [escolha UMA prioridade da lista acima]
PODE_RESOLVER: [SIM ou NÃO - se o usuário consegue resolver sozinho com suas instruções]
SOLUÇÃO:
[Forneça uma solução clara e objetiva em até 150 palavras. Se PODE_RESOLVER for SIM, dê instruções passo a passo. Se for NÃO, explique brevemente o que precisa ser feito e por que necessita de um técnico.]

IMPORTANTE:
- Para CATEGORIA, use EXATAMENTE um dos nomes da lista
- Para PRIORIDADE, use EXATAMENTE um dos níveis da lista
- Para PODE_RESOLVER, use apenas SIM ou NÃO
- Seja direto e prático na SOLUÇÃO";

            // Chamar API OpenAI
            var chatCompletion = await client.CompleteChatAsync(prompt);
            var respostaIA = chatCompletion.Value.Content[0].Text;

            _logger.LogInformation("Resposta da IA recebida: {Resposta}", respostaIA);

            // Parsear resposta da IA
            var (categoria, prioridade, podeResolver, solucao) = ParsearRespostaIA(respostaIA, categorias, prioridades);

            // Montar DTO de resposta
            var resultado = new AnalisePreliminarDto
            {
                SugestaoSolucao = solucao,
                CategoriaSugerida = new CategoriaDto
                {
                    IdCategoria = categoria.IdCategoria,
                    Nome = categoria.NomeCategoria,
                    Descricao = categoria.Descricao
                },
                PrioridadeSugerida = new PrioridadeDto
                {
                    IdPrioridade = prioridade.IdPrioridade,
                    Nivel = prioridade.NivelPrioridade,
                    Descricao = prioridade.Descricao
                },
                PodeResolverSemTecnico = podeResolver,
                NivelConfianca = "Alta"
            };

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao analisar problema com IA");
            return StatusCode(500, new { erro = "Erro ao processar análise de IA", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Parseia a resposta da IA e valida contra categorias/prioridades do banco
    /// </summary>
    /// <remarks>
    /// Extrai as seguintes informações da resposta da IA:
    /// - CATEGORIA: nome da categoria sugerida
    /// - PRIORIDADE: nível da prioridade sugerida
    /// - PODE_RESOLVER: SIM ou NÃO
    /// - SOLUÇÃO: texto com instruções
    ///
    /// Valida se categoria e prioridade existem no banco.
    /// Usa fallback para "Outros" e "Média" se não encontrar.
    /// </remarks>
    /// <param name="respostaIA">Texto retornado pela OpenAI</param>
    /// <param name="categorias">Lista de categorias disponíveis no sistema</param>
    /// <param name="prioridades">Lista de prioridades disponíveis no sistema</param>
    /// <returns>Tupla com categoria, prioridade, indicador booleano e texto da solução</returns>
    private (Entities.Categoria categoria, Entities.Prioridade prioridade, bool podeResolver, string solucao)
        ParsearRespostaIA(string respostaIA, List<Entities.Categoria> categorias, List<Entities.Prioridade> prioridades)
    {
        var linhas = respostaIA.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        string? categoriaNome = null;
        string? prioridadeNome = null;
        bool podeResolver = false;
        string solucao = "";

        bool capturandoSolucao = false;

        foreach (var linha in linhas)
        {
            var linhaLimpa = linha.Trim();

            if (linhaLimpa.StartsWith("CATEGORIA:", StringComparison.OrdinalIgnoreCase))
            {
                categoriaNome = linhaLimpa.Substring("CATEGORIA:".Length).Trim();
            }
            else if (linhaLimpa.StartsWith("PRIORIDADE:", StringComparison.OrdinalIgnoreCase))
            {
                prioridadeNome = linhaLimpa.Substring("PRIORIDADE:".Length).Trim();
            }
            else if (linhaLimpa.StartsWith("PODE_RESOLVER:", StringComparison.OrdinalIgnoreCase))
            {
                var valor = linhaLimpa.Substring("PODE_RESOLVER:".Length).Trim().ToUpper();
                podeResolver = valor.Contains("SIM");
            }
            else if (linhaLimpa.StartsWith("SOLUÇÃO:", StringComparison.OrdinalIgnoreCase) ||
                     linhaLimpa.StartsWith("SOLUCAO:", StringComparison.OrdinalIgnoreCase))
            {
                capturandoSolucao = true;
                // Capturar texto após "SOLUÇÃO:" se houver
                var resto = linhaLimpa.Contains(":") ? linhaLimpa.Substring(linhaLimpa.IndexOf(':') + 1).Trim() : "";
                if (!string.IsNullOrWhiteSpace(resto))
                {
                    solucao = resto;
                }
            }
            else if (capturandoSolucao && !string.IsNullOrWhiteSpace(linhaLimpa))
            {
                solucao += (string.IsNullOrWhiteSpace(solucao) ? "" : "\n") + linhaLimpa;
            }
        }

        // Buscar categoria correspondente (case-insensitive)
        var categoria = categorias.FirstOrDefault(c =>
            c.NomeCategoria.Equals(categoriaNome, StringComparison.OrdinalIgnoreCase));

        if (categoria == null)
        {
            _logger.LogWarning("Categoria '{Categoria}' não encontrada, usando 'Outros' como fallback", categoriaNome);
            categoria = categorias.FirstOrDefault(c => c.NomeCategoria == "Outros") ?? categorias.First();
        }

        // Buscar prioridade correspondente (case-insensitive)
        var prioridade = prioridades.FirstOrDefault(p =>
            p.NivelPrioridade.Equals(prioridadeNome, StringComparison.OrdinalIgnoreCase));

        if (prioridade == null)
        {
            _logger.LogWarning("Prioridade '{Prioridade}' não encontrada, usando 'Média' como fallback", prioridadeNome);
            prioridade = prioridades.FirstOrDefault(p => p.NivelPrioridade == "Média") ?? prioridades.First();
        }

        // Se solução ficou vazia, usar mensagem padrão
        if (string.IsNullOrWhiteSpace(solucao))
        {
            solucao = "Não foi possível gerar uma sugestão específica para este problema. Um técnico especializado poderá ajudá-lo melhor.";
            podeResolver = false;
        }

        return (categoria, prioridade, podeResolver, solucao);
    }
}
