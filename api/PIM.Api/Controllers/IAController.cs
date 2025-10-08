using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using PIM.Api.Data;
using PIM.Api.Entities;

namespace PIM.Api.Controllers;

[ApiController]
[Route("api/chamados/{idChamado}/ia")]
[Authorize]
public class IAController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public IAController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Gera sugestão completa de análise do chamado usando IA
    /// </summary>
    [HttpPost("sugestao")]
    public async Task<IActionResult> GerarSugestaoCompleta(int idChamado)
    {
        try
        {
            var chamado = await _context.Chamados
                .Include(c => c.Categoria)
                .Include(c => c.Prioridade)
                .FirstOrDefaultAsync(c => c.IdChamado == idChamado);

            if (chamado == null)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            // Verificar se já existe sugestão recente (últimas 24h)
            var sugestaoRecente = await _context.SugestoesIA
                .Where(s => s.IdChamado == idChamado && s.DataHoraGeracao > DateTime.UtcNow.AddHours(-24))
                .OrderByDescending(s => s.DataHoraGeracao)
                .FirstOrDefaultAsync();

            if (sugestaoRecente != null)
            {
                return Ok(new
                {
                    id = sugestaoRecente.IdSugestao,
                    idChamado = sugestaoRecente.IdChamado,
                    textoSugestao = sugestaoRecente.TextoSugestao,
                    dataHoraGeracao = sugestaoRecente.DataHoraGeracao,
                    foiAceita = sugestaoRecente.FoiAceita,
                    mensagem = "Sugestão gerada nas últimas 24 horas"
                });
            }

            // Obter chave da API OpenAI
            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new { erro = "Chave da API OpenAI não configurada" });
            }

            // Criar cliente OpenAI
            var client = new ChatClient("gpt-3.5-turbo", apiKey);

            // Montar prompt para a IA
            var prompt = $@"Você é um assistente de suporte técnico especializado. Analise o seguinte chamado e forneça uma sugestão de solução:

TÍTULO: {chamado.Titulo}
DESCRIÇÃO: {chamado.Descricao}
CATEGORIA ATUAL: {chamado.Categoria?.NomeCategoria ?? "Não definida"}
PRIORIDADE ATUAL: {chamado.Prioridade?.NivelPrioridade ?? "Não definida"}

Com base nessas informações, forneça:
1. Uma análise breve do problema (máximo 2 linhas)
2. Passos sugeridos para solução (máximo 4 passos)
3. Se necessário, recomendações de categoria ou prioridade mais adequadas

Seja direto e objetivo. Máximo 200 palavras.";

            // Chamar API OpenAI
            var chatCompletion = await client.CompleteChatAsync(prompt);
            var respostaIA = chatCompletion.Value.Content[0].Text;

            // Salvar sugestão no banco
            var sugestao = new SugestaoIA
            {
                IdChamado = idChamado,
                TextoSugestao = respostaIA,
                DataHoraGeracao = DateTime.UtcNow,
                FoiAceita = false
            };

            _context.SugestoesIA.Add(sugestao);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = sugestao.IdSugestao,
                idChamado = sugestao.IdChamado,
                textoSugestao = sugestao.TextoSugestao,
                dataHoraGeracao = sugestao.DataHoraGeracao,
                foiAceita = sugestao.FoiAceita
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar sugestão de IA", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// IA sugere a categoria mais adequada para o chamado
    /// </summary>
    [HttpPost("categorizar")]
    public async Task<IActionResult> CategorizarChamado(int idChamado)
    {
        try
        {
            var chamado = await _context.Chamados
                .FirstOrDefaultAsync(c => c.IdChamado == idChamado);

            if (chamado == null)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            // Obter todas as categorias disponíveis
            var categorias = await _context.Categorias.ToListAsync();
            var listaCategoriasTexto = string.Join(", ", categorias.Select(c => c.NomeCategoria));

            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new { erro = "Chave da API OpenAI não configurada" });
            }

            var client = new ChatClient("gpt-3.5-turbo", apiKey);

            var prompt = $@"Você é um classificador de chamados de suporte técnico.

TÍTULO: {chamado.Titulo}
DESCRIÇÃO: {chamado.Descricao}

CATEGORIAS DISPONÍVEIS: {listaCategoriasTexto}

Responda APENAS com o nome exato de UMA categoria da lista acima que melhor se encaixa neste chamado.
Não adicione explicações, apenas o nome da categoria.";

            var chatCompletion = await client.CompleteChatAsync(prompt);
            var categoriaSugerida = chatCompletion.Value.Content[0].Text.Trim();

            // Buscar categoria sugerida no banco
            var categoria = categorias.FirstOrDefault(c =>
                c.NomeCategoria.Equals(categoriaSugerida, StringComparison.OrdinalIgnoreCase));

            if (categoria == null)
            {
                // Fallback: retornar "Outros"
                categoria = categorias.FirstOrDefault(c => c.NomeCategoria == "Outros");
            }

            return Ok(new
            {
                categoriaSugerida = categoria?.NomeCategoria ?? categoriaSugerida,
                idCategoria = categoria?.IdCategoria,
                confianca = categoria != null ? "Alta" : "Baixa (categoria não encontrada)"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao categorizar chamado", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// IA sugere a prioridade mais adequada para o chamado
    /// </summary>
    [HttpPost("priorizar")]
    public async Task<IActionResult> PriorizarChamado(int idChamado)
    {
        try
        {
            var chamado = await _context.Chamados
                .FirstOrDefaultAsync(c => c.IdChamado == idChamado);

            if (chamado == null)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            var prioridades = await _context.Prioridades.ToListAsync();
            var listaPrioridadesTexto = string.Join(", ", prioridades.Select(p => p.NivelPrioridade));

            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new { erro = "Chave da API OpenAI não configurada" });
            }

            var client = new ChatClient("gpt-3.5-turbo", apiKey);

            var prompt = $@"Você é um classificador de prioridade de chamados de suporte técnico.

TÍTULO: {chamado.Titulo}
DESCRIÇÃO: {chamado.Descricao}

PRIORIDADES DISPONÍVEIS: {listaPrioridadesTexto}

Analise o impacto e urgência do problema. Responda APENAS com o nome exato de UMA prioridade da lista acima.
Não adicione explicações, apenas o nome da prioridade.

Critérios:
- Crítica: Sistema parado, múltiplos usuários afetados, perda de produtividade massiva
- Alta: Impossibilita trabalho individual, urgente
- Média: Dificulta trabalho mas não impede
- Baixa: Não afeta trabalho imediato";

            var chatCompletion = await client.CompleteChatAsync(prompt);
            var prioridadeSugerida = chatCompletion.Value.Content[0].Text.Trim();

            var prioridade = prioridades.FirstOrDefault(p =>
                p.NivelPrioridade.Equals(prioridadeSugerida, StringComparison.OrdinalIgnoreCase));

            return Ok(new
            {
                prioridadeSugerida = prioridade?.NivelPrioridade ?? prioridadeSugerida,
                idPrioridade = prioridade?.IdPrioridade,
                confianca = prioridade != null ? "Alta" : "Baixa (prioridade não encontrada)"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao priorizar chamado", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Lista todas as sugestões de IA para um chamado
    /// </summary>
    [HttpGet("sugestoes")]
    public async Task<IActionResult> ListarSugestoes(int idChamado)
    {
        try
        {
            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == idChamado);
            if (!chamadoExiste)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            var sugestoes = await _context.SugestoesIA
                .Where(s => s.IdChamado == idChamado)
                .OrderByDescending(s => s.DataHoraGeracao)
                .Take(10)
                .Select(s => new
                {
                    id = s.IdSugestao,
                    textoSugestao = s.TextoSugestao,
                    dataHoraGeracao = s.DataHoraGeracao,
                    foiAceita = s.FoiAceita
                })
                .ToListAsync();

            return Ok(sugestoes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao listar sugestões", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Marca uma sugestão de IA como aceita ou rejeitada
    /// </summary>
    [HttpPut("sugestoes/{idSugestao}/aceitar")]
    public async Task<IActionResult> AceitarSugestao(int idChamado, int idSugestao, [FromBody] AceitarSugestaoRequest request)
    {
        try
        {
            var sugestao = await _context.SugestoesIA
                .FirstOrDefaultAsync(s => s.IdSugestao == idSugestao && s.IdChamado == idChamado);

            if (sugestao == null)
            {
                return NotFound(new { erro = "Sugestão não encontrada" });
            }

            sugestao.FoiAceita = request.Aceita;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = sugestao.IdSugestao,
                idChamado = sugestao.IdChamado,
                foiAceita = sugestao.FoiAceita,
                mensagem = request.Aceita ? "Sugestão aceita com sucesso" : "Sugestão rejeitada"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao atualizar sugestão", detalhes = ex.Message });
        }
    }
}

public class AceitarSugestaoRequest
{
    public bool Aceita { get; set; }
}
