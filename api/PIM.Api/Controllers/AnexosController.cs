using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.Entities;

namespace PIM.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class AnexosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<AnexosController> _logger;

    // Configurações de upload
    private readonly long _maxFileSize = 10 * 1024 * 1024; // 10 MB
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".txt", ".doc", ".docx", ".zip", ".rar" };

    public AnexosController(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<AnexosController> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Upload de anexo para um chamado
    /// </summary>
    [HttpPost("chamados/{idChamado}/anexos")]
    public async Task<IActionResult> UploadAnexo(int idChamado, IFormFile arquivo)
    {
        try
        {
            // Validar se o chamado existe
            var chamado = await _context.Chamados.FindAsync(idChamado);
            if (chamado == null)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            // Validar se o arquivo foi enviado
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest(new { erro = "Nenhum arquivo foi enviado" });
            }

            // Validar tamanho do arquivo
            if (arquivo.Length > _maxFileSize)
            {
                return BadRequest(new { erro = $"Arquivo muito grande. Tamanho máximo permitido: {_maxFileSize / 1024 / 1024} MB" });
            }

            // Validar extensão do arquivo
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extensao))
            {
                return BadRequest(new
                {
                    erro = "Tipo de arquivo não permitido",
                    extensoesPermitidas = _allowedExtensions
                });
            }

            // Criar estrutura de diretórios: wwwroot/uploads/{ano}/{mes}/
            var dataAtual = DateTime.UtcNow;
            var pastaAno = dataAtual.Year.ToString();
            var pastaMes = dataAtual.Month.ToString("D2");
            var caminhoRelativo = Path.Combine("uploads", pastaAno, pastaMes);
            var caminhoCompleto = Path.Combine(_environment.WebRootPath, caminhoRelativo);

            // Criar diretório se não existir
            if (!Directory.Exists(caminhoCompleto))
            {
                Directory.CreateDirectory(caminhoCompleto);
            }

            // Gerar nome único para o arquivo
            var nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            var nomeUnico = $"{Guid.NewGuid()}_{nomeOriginal}{extensao}";
            var caminhoArquivo = Path.Combine(caminhoCompleto, nomeUnico);

            // Salvar arquivo no disco
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // Salvar registro no banco de dados
            var anexo = new Anexo
            {
                IdChamado = idChamado,
                NomeArquivo = arquivo.FileName,
                CaminhoArquivo = Path.Combine(caminhoRelativo, nomeUnico).Replace("\\", "/"),
                DataUpload = DateTime.UtcNow
            };

            _context.Anexos.Add(anexo);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Anexo {anexo.IdAnexo} salvo para chamado {idChamado}");

            return CreatedAtAction(
                nameof(DownloadAnexo),
                new { id = anexo.IdAnexo },
                new
                {
                    id = anexo.IdAnexo,
                    idChamado = anexo.IdChamado,
                    nomeArquivo = anexo.NomeArquivo,
                    tamanhoBytes = arquivo.Length,
                    dataUpload = anexo.DataUpload,
                    urlDownload = $"/api/anexos/{anexo.IdAnexo}/download"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao fazer upload de anexo para chamado {idChamado}");
            return StatusCode(500, new { erro = "Erro ao fazer upload do arquivo", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Listar anexos de um chamado
    /// </summary>
    [HttpGet("chamados/{idChamado}/anexos")]
    public async Task<IActionResult> ListarAnexosChamado(int idChamado)
    {
        try
        {
            // Validar se o chamado existe
            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == idChamado);
            if (!chamadoExiste)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            // Buscar anexos
            var anexos = await _context.Anexos
                .Where(a => a.IdChamado == idChamado)
                .OrderByDescending(a => a.DataUpload)
                .Select(a => new
                {
                    id = a.IdAnexo,
                    nomeArquivo = a.NomeArquivo,
                    dataUpload = a.DataUpload,
                    urlDownload = $"/api/anexos/{a.IdAnexo}/download",
                    extensao = Path.GetExtension(a.NomeArquivo).ToLowerInvariant()
                })
                .ToListAsync();

            return Ok(new
            {
                idChamado,
                totalAnexos = anexos.Count,
                anexos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao listar anexos do chamado {idChamado}");
            return StatusCode(500, new { erro = "Erro ao listar anexos", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Download de um anexo
    /// </summary>
    [HttpGet("anexos/{id}/download")]
    public async Task<IActionResult> DownloadAnexo(int id)
    {
        try
        {
            // Buscar anexo no banco
            var anexo = await _context.Anexos.FindAsync(id);
            if (anexo == null)
            {
                return NotFound(new { erro = "Anexo não encontrado" });
            }

            // Montar caminho completo do arquivo
            var caminhoCompleto = Path.Combine(_environment.WebRootPath, anexo.CaminhoArquivo);

            // Verificar se o arquivo existe no disco
            if (!System.IO.File.Exists(caminhoCompleto))
            {
                _logger.LogWarning($"Arquivo físico não encontrado: {caminhoCompleto}");
                return NotFound(new { erro = "Arquivo físico não encontrado no servidor" });
            }

            // Ler arquivo
            var memoria = new MemoryStream();
            using (var stream = new FileStream(caminhoCompleto, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memoria);
            }
            memoria.Position = 0;

            // Determinar content type baseado na extensão
            var extensao = Path.GetExtension(anexo.NomeArquivo).ToLowerInvariant();
            var contentType = extensao switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };

            _logger.LogInformation($"Download do anexo {id} - {anexo.NomeArquivo}");

            return File(memoria, contentType, anexo.NomeArquivo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao fazer download do anexo {id}");
            return StatusCode(500, new { erro = "Erro ao fazer download do arquivo", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Remover um anexo (deleta arquivo do disco e registro do banco)
    /// </summary>
    [HttpDelete("anexos/{id}")]
    public async Task<IActionResult> RemoverAnexo(int id)
    {
        try
        {
            // Buscar anexo no banco
            var anexo = await _context.Anexos
                .Include(a => a.Chamado)
                .FirstOrDefaultAsync(a => a.IdAnexo == id);

            if (anexo == null)
            {
                return NotFound(new { erro = "Anexo não encontrado" });
            }

            // Validar permissão: apenas Admin ou o solicitante do chamado podem remover
            var userId = User.FindFirst("id")?.Value;
            var userRole = User.FindFirst("role")?.Value;

            if (userRole != "Admin" && anexo.Chamado.IdUsuarioCriador != int.Parse(userId ?? "0"))
            {
                return StatusCode(403, new { erro = "Você não tem permissão para remover este anexo" });
            }

            // Tentar remover arquivo físico do disco
            var caminhoCompleto = Path.Combine(_environment.WebRootPath, anexo.CaminhoArquivo);
            if (System.IO.File.Exists(caminhoCompleto))
            {
                System.IO.File.Delete(caminhoCompleto);
                _logger.LogInformation($"Arquivo físico removido: {caminhoCompleto}");
            }
            else
            {
                _logger.LogWarning($"Arquivo físico não encontrado ao tentar remover: {caminhoCompleto}");
            }

            // Remover registro do banco
            _context.Anexos.Remove(anexo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Anexo removido com sucesso",
                idAnexo = id,
                nomeArquivo = anexo.NomeArquivo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao remover anexo {id}");
            return StatusCode(500, new { erro = "Erro ao remover anexo", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Obter informações de um anexo específico (sem fazer download)
    /// </summary>
    [HttpGet("anexos/{id}")]
    public async Task<IActionResult> GetAnexo(int id)
    {
        try
        {
            var anexo = await _context.Anexos
                .Include(a => a.Chamado)
                .FirstOrDefaultAsync(a => a.IdAnexo == id);

            if (anexo == null)
            {
                return NotFound(new { erro = "Anexo não encontrado" });
            }

            // Verificar se arquivo existe no disco
            var caminhoCompleto = Path.Combine(_environment.WebRootPath, anexo.CaminhoArquivo);
            var arquivoExiste = System.IO.File.Exists(caminhoCompleto);
            long tamanhoBytes = 0;

            if (arquivoExiste)
            {
                var fileInfo = new FileInfo(caminhoCompleto);
                tamanhoBytes = fileInfo.Length;
            }

            return Ok(new
            {
                id = anexo.IdAnexo,
                idChamado = anexo.IdChamado,
                tituloChamado = anexo.Chamado.Titulo,
                nomeArquivo = anexo.NomeArquivo,
                extensao = Path.GetExtension(anexo.NomeArquivo).ToLowerInvariant(),
                tamanhoBytes,
                tamanhoFormatado = FormatarTamanho(tamanhoBytes),
                dataUpload = anexo.DataUpload,
                urlDownload = $"/api/anexos/{anexo.IdAnexo}/download",
                arquivoExisteNoDisco = arquivoExiste
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao obter informações do anexo {id}");
            return StatusCode(500, new { erro = "Erro ao obter informações do anexo", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Formatar tamanho de arquivo em formato legível
    /// </summary>
    private string FormatarTamanho(long bytes)
    {
        string[] tamanhos = { "B", "KB", "MB", "GB" };
        double tamanho = bytes;
        int ordem = 0;

        while (tamanho >= 1024 && ordem < tamanhos.Length - 1)
        {
            ordem++;
            tamanho /= 1024;
        }

        return $"{tamanho:0.##} {tamanhos[ordem]}";
    }
}
