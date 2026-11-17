using Microsoft.EntityFrameworkCore;
using PIM.Api.Entities;
using PIM.Api.Utils;

namespace PIM.Api.Data
{
    /// <summary>
    /// Classe para popular o banco com dados iniciais conforme DOCsimplificado.md
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Cria os cadastros básicos (perfis, setores, categorias, prioridades, status e usuários).
        /// </summary>
        /// <param name="incluirDadosDeExemplo">
        /// Quando verdadeiro, cria também os chamados de exemplo de <see cref="DadosDeExemplo"/>.
        /// </param>
        public static async Task SeedAsync(ApplicationDbContext context, bool incluirDadosDeExemplo = false)
        {
            // Com dados de exemplo, os usuários precisam existir antes dos chamados antigos que eles abriram.
            var dataCadastro = incluirDadosDeExemplo ? DateTime.Now.AddDays(-150) : DateTime.Now;

            // Seed Perfis de Acesso
            if (!await context.PerfisAcesso.AnyAsync())
            {
                var perfis = new[]
                {
                    new PerfilAcesso { NomePerfil = "Administrador", Descricao = "Acesso completo ao sistema, incluindo configurações e relatórios" },
                    new PerfilAcesso { NomePerfil = "Técnico", Descricao = "Perfil para os profissionais que atendem aos chamados" },
                    new PerfilAcesso { NomePerfil = "Usuário", Descricao = "Perfil básico para abertura e acompanhamento de chamados" }
                };
                context.PerfisAcesso.AddRange(perfis);
                await context.SaveChangesAsync();
            }

            // Seed Setores
            if (!await context.Setores.AnyAsync())
            {
                var setores = new[]
                {
                    new Setor { NomeSetor = "TI", Descricao = "Setor responsável pela infraestrutura de TI e suporte técnico" },
                    new Setor { NomeSetor = "RH", Descricao = "Setor responsável pela gestão de recursos humanos" },
                    new Setor { NomeSetor = "Financeiro", Descricao = "Setor responsável pelas finanças e contabilidade" },
                    new Setor { NomeSetor = "Comercial", Descricao = "Setor responsável pelas vendas e atendimento ao cliente" },
                    new Setor { NomeSetor = "Administrativo", Descricao = "Setor responsável pela administração geral da empresa" }
                };
                context.Setores.AddRange(setores);
                await context.SaveChangesAsync();
            }

            // Seed Categorias
            if (!await context.Categorias.AnyAsync())
            {
                var categorias = new[]
                {
                    new Categoria { NomeCategoria = "Hardware", Descricao = "Problemas relacionados a equipamentos físicos" },
                    new Categoria { NomeCategoria = "Software", Descricao = "Problemas relacionados a programas e sistemas" },
                    new Categoria { NomeCategoria = "Rede", Descricao = "Problemas relacionados à conectividade e internet" },
                    new Categoria { NomeCategoria = "Email", Descricao = "Problemas relacionados a contas e acesso a emails" },
                    new Categoria { NomeCategoria = "Impressão", Descricao = "Problemas relacionados a impressoras e scanners" },
                    new Categoria { NomeCategoria = "Segurança", Descricao = "Problemas relacionados a senhas e acessos" },
                    new Categoria { NomeCategoria = "Outros", Descricao = "Problemas não categorizados nas opções anteriores" }
                };
                context.Categorias.AddRange(categorias);
                await context.SaveChangesAsync();
            }

            // Seed Prioridades
            if (!await context.Prioridades.AnyAsync())
            {
                var prioridades = new[]
                {
                    new Prioridade { NivelPrioridade = "Baixa", Descricao = "Problemas que não afetam diretamente o trabalho" },
                    new Prioridade { NivelPrioridade = "Média", Descricao = "Problemas que dificultam, mas não impedem o trabalho" },
                    new Prioridade { NivelPrioridade = "Alta", Descricao = "Problemas que impedem o trabalho individual" },
                    new Prioridade { NivelPrioridade = "Crítica", Descricao = "Problemas que afetam múltiplos usuários ou sistemas críticos" }
                };
                context.Prioridades.AddRange(prioridades);
                await context.SaveChangesAsync();
            }

            // Seed Status (conforme documentação - 8 status)
            if (!await context.StatusChamados.AnyAsync())
            {
                var status = new[]
                {
                    new StatusChamado { NomeStatus = "Aberto", Descricao = "Chamado registrado, aguardando atribuição" },
                    new StatusChamado { NomeStatus = "Em Análise", Descricao = "Chamado em análise pelo técnico" },
                    new StatusChamado { NomeStatus = "Em Andamento", Descricao = "Chamado sendo atendido pelo técnico" },
                    new StatusChamado { NomeStatus = "Aguardando Usuário", Descricao = "Aguardando retorno do usuário" },
                    new StatusChamado { NomeStatus = "Aguardando Fornecedor", Descricao = "Aguardando resposta ou peça de fornecedor" },
                    new StatusChamado { NomeStatus = "Resolvido", Descricao = "Solução implementada, aguardando confirmação" },
                    new StatusChamado { NomeStatus = "Fechado", Descricao = "Chamado concluído e aprovado pelo usuário" },
                    new StatusChamado { NomeStatus = "Cancelado", Descricao = "Chamado cancelado a pedido do usuário" }
                };
                context.StatusChamados.AddRange(status);
                await context.SaveChangesAsync();
            }

            // Seed Usuários de exemplo (conforme documentação - 8 usuários)
            if (!await context.Usuarios.AnyAsync())
            {
                var perfilAdmin = await context.PerfisAcesso.FirstAsync(p => p.NomePerfil == "Administrador");
                var perfilTecnico = await context.PerfisAcesso.FirstAsync(p => p.NomePerfil == "Técnico");
                var perfilUsuario = await context.PerfisAcesso.FirstAsync(p => p.NomePerfil == "Usuário");

                var setorTI = await context.Setores.FirstAsync(s => s.NomeSetor == "TI");
                var setorRH = await context.Setores.FirstAsync(s => s.NomeSetor == "RH");
                var setorFinanceiro = await context.Setores.FirstAsync(s => s.NomeSetor == "Financeiro");
                var setorComercial = await context.Setores.FirstAsync(s => s.NomeSetor == "Comercial");
                var setorAdministrativo = await context.Setores.FirstAsync(s => s.NomeSetor == "Administrativo");

                var usuarios = new[]
                {
                    new Usuario
                    {
                        NomeCompleto = "Admin Sistema",
                        Email = "admin@empresa.com",
                        Senha = PasswordHelper.HashPassword("admin123"),
                        Telefone = "(11) 98765-4321",
                        IdSetor = setorTI.IdSetor,
                        IdPerfil = perfilAdmin.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = false,
                        Ativo = true
                    },
                    new Usuario
                    {
                        NomeCompleto = "João Silva",
                        Email = "joao.silva@empresa.com",
                        Senha = PasswordHelper.HashPassword("senha123"),
                        Telefone = "(11) 97654-3210",
                        IdSetor = setorTI.IdSetor,
                        IdPerfil = perfilTecnico.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = false,
                        Ativo = true
                    },
                    new Usuario
                    {
                        NomeCompleto = "Maria Santos",
                        Email = "maria.santos@empresa.com",
                        Senha = PasswordHelper.HashPassword("senha123"),
                        Telefone = "(11) 96543-2109",
                        IdSetor = setorTI.IdSetor,
                        IdPerfil = perfilTecnico.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = false,
                        Ativo = true
                    },
                    new Usuario
                    {
                        NomeCompleto = "Carlos Oliveira",
                        Email = "carlos.oliveira@empresa.com",
                        Senha = PasswordHelper.HashPassword("senha123"),
                        Telefone = "(11) 95432-1098",
                        IdSetor = setorTI.IdSetor,
                        IdPerfil = perfilAdmin.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = false,
                        Ativo = true
                    },
                    new Usuario
                    {
                        NomeCompleto = "Ana Souza",
                        Email = "ana.souza@empresa.com",
                        Senha = PasswordHelper.HashPassword("senha123"),
                        Telefone = "(11) 94321-0987",
                        IdSetor = setorRH.IdSetor,
                        IdPerfil = perfilUsuario.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = true,
                        Ativo = true
                    },
                    new Usuario
                    {
                        NomeCompleto = "Pedro Costa",
                        Email = "pedro.costa@empresa.com",
                        Senha = PasswordHelper.HashPassword("senha123"),
                        Telefone = "(11) 93210-9876",
                        IdSetor = setorFinanceiro.IdSetor,
                        IdPerfil = perfilUsuario.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = false,
                        Ativo = true
                    },
                    new Usuario
                    {
                        NomeCompleto = "Lucia Ferreira",
                        Email = "lucia.ferreira@empresa.com",
                        Senha = PasswordHelper.HashPassword("senha123"),
                        Telefone = "(11) 92109-8765",
                        IdSetor = setorComercial.IdSetor,
                        IdPerfil = perfilUsuario.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = false,
                        Ativo = true
                    },
                    new Usuario
                    {
                        NomeCompleto = "Roberto Almeida",
                        Email = "roberto.almeida@empresa.com",
                        Senha = PasswordHelper.HashPassword("senha123"),
                        Telefone = "(11) 91098-7654",
                        IdSetor = setorAdministrativo.IdSetor,
                        IdPerfil = perfilUsuario.IdPerfil,
                        DataCadastro = dataCadastro,
                        PrimeiroAcesso = false,
                        Ativo = true
                    }
                };

                context.Usuarios.AddRange(usuarios);
                await context.SaveChangesAsync();
            }

            if (incluirDadosDeExemplo)
            {
                await DadosDeExemplo.SeedAsync(context);
            }
        }
    }
}
