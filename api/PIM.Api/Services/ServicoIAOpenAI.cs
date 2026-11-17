using OpenAI.Chat;

namespace PIM.Api.Services;

/// <summary>
/// Implementação do serviço de IA com o modelo GPT-3.5-turbo da OpenAI.
/// </summary>
/// <remarks>
/// Exige internet e a chave configurada em "OpenAI:ApiKey".
/// Só é usada quando "IA:Modo" está como "OpenAI".
/// </remarks>
public class ServicoIAOpenAI : IServicoIA
{
    private const string Modelo = "gpt-3.5-turbo";
    private readonly string _apiKey;

    public ServicoIAOpenAI(string apiKey)
    {
        _apiKey = apiKey;
    }

    /// <summary>
    /// Indica se o valor informado parece uma chave de verdade, e não o texto de exemplo do appsettings.
    /// </summary>
    public static bool ChaveValida(string? apiKey) =>
        !string.IsNullOrWhiteSpace(apiKey) && apiKey.StartsWith("sk-", StringComparison.Ordinal);

    public Task<string> GerarSugestaoAsync(string titulo, string descricao, string? categoriaAtual, string? prioridadeAtual)
    {
        var prompt = $@"Você é um assistente de suporte técnico especializado. Analise o seguinte chamado e forneça uma sugestão de solução:

TÍTULO: {titulo}
DESCRIÇÃO: {descricao}
CATEGORIA ATUAL: {categoriaAtual ?? "Não definida"}
PRIORIDADE ATUAL: {prioridadeAtual ?? "Não definida"}

Com base nessas informações, forneça:
1. Uma análise breve do problema (máximo 2 linhas)
2. Passos sugeridos para solução (máximo 4 passos)
3. Se necessário, recomendações de categoria ou prioridade mais adequadas

Seja direto e objetivo. Máximo 200 palavras.";

        return CompletarAsync(prompt);
    }

    public async Task<string> CategorizarAsync(string titulo, string descricao, IReadOnlyList<string> categorias)
    {
        var prompt = $@"Você é um classificador de chamados de suporte técnico.

TÍTULO: {titulo}
DESCRIÇÃO: {descricao}

CATEGORIAS DISPONÍVEIS: {string.Join(", ", categorias)}

Responda APENAS com o nome exato de UMA categoria da lista acima que melhor se encaixa neste chamado.
Não adicione explicações, apenas o nome da categoria.";

        return (await CompletarAsync(prompt)).Trim();
    }

    public async Task<string> PriorizarAsync(string titulo, string descricao, IReadOnlyList<string> prioridades)
    {
        var prompt = $@"Você é um classificador de prioridade de chamados de suporte técnico.

TÍTULO: {titulo}
DESCRIÇÃO: {descricao}

PRIORIDADES DISPONÍVEIS: {string.Join(", ", prioridades)}

Analise o impacto e urgência do problema. Responda APENAS com o nome exato de UMA prioridade da lista acima.
Não adicione explicações, apenas o nome da prioridade.

Critérios:
- Crítica: Sistema parado, múltiplos usuários afetados, perda de produtividade massiva
- Alta: Impossibilita trabalho individual, urgente
- Média: Dificulta trabalho mas não impede
- Baixa: Não afeta trabalho imediato";

        return (await CompletarAsync(prompt)).Trim();
    }

    public Task<string> AnalisarPreliminarAsync(string titulo, string descricao, IReadOnlyList<string> categorias, IReadOnlyList<string> prioridades)
    {
        var prompt = $@"Você é um assistente de suporte técnico inteligente. Analise o seguinte problema e forneça uma resposta estruturada.

TÍTULO: {titulo}
DESCRIÇÃO: {descricao}

CATEGORIAS DISPONÍVEIS: {string.Join(", ", categorias)}
PRIORIDADES DISPONÍVEIS: {string.Join(", ", prioridades)}

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

        return CompletarAsync(prompt);
    }

    private async Task<string> CompletarAsync(string prompt)
    {
        var client = new ChatClient(Modelo, _apiKey);
        var resposta = await client.CompleteChatAsync(prompt);
        return resposta.Value.Content[0].Text;
    }
}
