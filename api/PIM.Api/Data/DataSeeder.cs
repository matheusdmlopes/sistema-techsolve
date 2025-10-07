using Microsoft.EntityFrameworkCore;
using PIM.Api.Entities;
using PIM.Api.Utils;

namespace PIM.Api.Data
{
    /// <summary>
    /// Classe para popular o banco com dados iniciais
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Perfis de Acesso
            if (!await context.PerfisAcesso.AnyAsync())
            {
                var perfis = new[]
                {
                    new PerfilAcesso { NomePerfil = "Admin", Descricao = "Administrador do sistema" },
                    new PerfilAcesso { NomePerfil = "Técnico", Descricao = "Técnico de suporte" },
                    new PerfilAcesso { NomePerfil = "Usuário", Descricao = "Usuário solicitante" }
                };
                context.PerfisAcesso.AddRange(perfis);
                await context.SaveChangesAsync();
            }

            // Seed Setores
            if (!await context.Setores.AnyAsync())
            {
                var setores = new[]
                {
                    new Setor { NomeSetor = "TI", Descricao = "Tecnologia da Informação" },
                    new Setor { NomeSetor = "RH", Descricao = "Recursos Humanos" },
                    new Setor { NomeSetor = "Financeiro", Descricao = "Setor Financeiro" },
                    new Setor { NomeSetor = "Vendas", Descricao = "Setor de Vendas" },
                    new Setor { NomeSetor = "Operações", Descricao = "Setor Operacional" }
                };
                context.Setores.AddRange(setores);
                await context.SaveChangesAsync();
            }

            // Seed Categorias
            if (!await context.Categorias.AnyAsync())
            {
                var categorias = new[]
                {
                    new Categoria { NomeCategoria = "Hardware", Descricao = "Problemas com equipamentos" },
                    new Categoria { NomeCategoria = "Software", Descricao = "Problemas com aplicativos" },
                    new Categoria { NomeCategoria = "Rede", Descricao = "Problemas de conectividade" },
                    new Categoria { NomeCategoria = "Acesso", Descricao = "Problemas de login e permissões" },
                    new Categoria { NomeCategoria = "Email", Descricao = "Problemas com email" },
                    new Categoria { NomeCategoria = "Impressora", Descricao = "Problemas com impressoras" },
                    new Categoria { NomeCategoria = "Outros", Descricao = "Outros tipos de problemas" }
                };
                context.Categorias.AddRange(categorias);
                await context.SaveChangesAsync();
            }

            // Seed Prioridades
            if (!await context.Prioridades.AnyAsync())
            {
                var prioridades = new[]
                {
                    new Prioridade { NivelPrioridade = "Baixa", Descricao = "Sem urgência" },
                    new Prioridade { NivelPrioridade = "Média", Descricao = "Urgência moderada" },
                    new Prioridade { NivelPrioridade = "Alta", Descricao = "Urgente" },
                    new Prioridade { NivelPrioridade = "Crítica", Descricao = "Extremamente urgente" }
                };
                context.Prioridades.AddRange(prioridades);
                await context.SaveChangesAsync();
            }

            // Seed Status
            if (!await context.StatusChamados.AnyAsync())
            {
                var status = new[]
                {
                    new StatusChamado { NomeStatus = "Aberto", Descricao = "Chamado recém criado" },
                    new StatusChamado { NomeStatus = "Em Atendimento", Descricao = "Chamado sendo atendido" },
                    new StatusChamado { NomeStatus = "Aguardando Usuário", Descricao = "Aguardando resposta do usuário" },
                    new StatusChamado { NomeStatus = "Aguardando Terceiros", Descricao = "Aguardando fornecedor externo" },
                    new StatusChamado { NomeStatus = "Resolvido", Descricao = "Problema resolvido" },
                    new StatusChamado { NomeStatus = "Fechado", Descricao = "Chamado encerrado" },
                    new StatusChamado { NomeStatus = "Cancelado", Descricao = "Chamado cancelado" }
                };
                context.StatusChamados.AddRange(status);
                await context.SaveChangesAsync();
            }

            // Seed Usuário Admin padrão
            if (!await context.Usuarios.AnyAsync())
            {
                var perfilAdmin = await context.PerfisAcesso.FirstAsync(p => p.NomePerfil == "Admin");
                var setorTI = await context.Setores.FirstAsync(s => s.NomeSetor == "TI");

                var admin = new Usuario
                {
                    NomeCompleto = "Administrador do Sistema",
                    Email = "admin@techsolve.com",
                    Senha = PasswordHelper.HashPassword("admin123"), // Senha padrão
                    Telefone = "(11) 99999-9999",
                    IdSetor = setorTI.IdSetor,
                    IdPerfil = perfilAdmin.IdPerfil,
                    DataCadastro = DateTime.UtcNow,
                    PrimeiroAcesso = false,
                    Ativo = true
                };

                context.Usuarios.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
