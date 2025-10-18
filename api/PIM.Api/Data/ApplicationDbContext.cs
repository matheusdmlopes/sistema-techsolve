using Microsoft.EntityFrameworkCore;
using PIM.Api.Entities;

namespace PIM.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets para todas as entidades
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Setor> Setores { get; set; }
    public DbSet<PerfilAcesso> PerfisAcesso { get; set; }
    public DbSet<Chamado> Chamados { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Prioridade> Prioridades { get; set; }
    public DbSet<StatusChamado> StatusChamados { get; set; }
    public DbSet<HistoricoChamado> HistoricosChamados { get; set; }
    public DbSet<SugestaoIA> SugestoesIA { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Anexo> Anexos { get; set; }
    public DbSet<ComentarioChamado> ComentariosChamados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configura��o das tabelas
        ConfigureUsuario(modelBuilder);
        ConfigureSetor(modelBuilder);
        ConfigurePerfilAcesso(modelBuilder);
        ConfigureChamado(modelBuilder);
        ConfigureCategoria(modelBuilder);
        ConfigurePrioridade(modelBuilder);
        ConfigureStatusChamado(modelBuilder);
        ConfigureHistoricoChamado(modelBuilder);
        ConfigureSugestaoIA(modelBuilder);
        ConfigureFeedback(modelBuilder);
        ConfigureAnexo(modelBuilder);
        ConfigureComentarioChamado(modelBuilder);
    }

    private static void ConfigureUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).ValueGeneratedOnAdd();
            
            entity.Property(e => e.NomeCompleto)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("UC_Usuario_Email");
            
            entity.Property(e => e.Senha)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.Telefone)
                .HasMaxLength(15);
            
            entity.Property(e => e.DataCadastro)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            
            entity.Property(e => e.PrimeiroAcesso)
                .IsRequired()
                .HasDefaultValue(true);
            
            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            // Relacionamentos
            entity.HasOne(e => e.Setor)
                .WithMany(s => s.Usuarios)
                .HasForeignKey(e => e.IdSetor)
                .HasConstraintName("FK_Usuario_Setor")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PerfilAcesso)
                .WithMany()
                .HasForeignKey(e => e.IdPerfil)
                .HasConstraintName("FK_Usuario_PerfilAcesso")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.ChamadosCriados)
                .WithOne(c => c.UsuarioCriador)
                .HasForeignKey(c => c.IdUsuarioCriador)
                .HasConstraintName("FK_Chamado_UsuarioCriador")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.ChamadosAtendidos)
                .WithOne(c => c.UsuarioTecnico)
                .HasForeignKey(c => c.IdUsuarioTecnico)
                .HasConstraintName("FK_Chamado_UsuarioTecnico")
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.HistoricosChamado)
                .WithOne(h => h.Tecnico)
                .HasForeignKey(h => h.IdTecnico)
                .HasConstraintName("FK_HistoricoChamado_Tecnico")
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureSetor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Setor>(entity =>
        {
            entity.ToTable("Setor");
            entity.HasKey(e => e.IdSetor);
            entity.Property(e => e.IdSetor).ValueGeneratedOnAdd();
            
            entity.Property(e => e.NomeSetor)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Descricao)
                .HasMaxLength(255);
        });
    }

    private static void ConfigurePerfilAcesso(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PerfilAcesso>(entity =>
        {
            entity.ToTable("PerfilAcesso");
            entity.HasKey(e => e.IdPerfil);
            entity.Property(e => e.IdPerfil).ValueGeneratedOnAdd();
            
            entity.Property(e => e.NomePerfil)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Descricao)
                .HasMaxLength(255);
        });
    }

    private static void ConfigureChamado(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chamado>(entity =>
        {
            entity.ToTable("Chamado");
            entity.HasKey(e => e.IdChamado);
            entity.Property(e => e.IdChamado).ValueGeneratedOnAdd();
            
            entity.Property(e => e.Titulo)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Descricao)
                .IsRequired()
                .HasColumnType("VARCHAR(MAX)");
            
            entity.Property(e => e.DataAbertura)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            
            entity.Property(e => e.SolucaoFinal)
                .HasColumnType("VARCHAR(MAX)");

            // Relacionamentos
            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Chamados)
                .HasForeignKey(e => e.IdCategoria)
                .HasConstraintName("FK_Chamado_Categoria")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Prioridade)
                .WithMany(p => p.Chamados)
                .HasForeignKey(e => e.IdPrioridade)
                .HasConstraintName("FK_Chamado_Prioridade")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Status)
                .WithMany(s => s.Chamados)
                .HasForeignKey(e => e.IdStatus)
                .HasConstraintName("FK_Chamado_Status")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Historicos)
                .WithOne(h => h.Chamado)
                .HasForeignKey(h => h.IdChamado)
                .HasConstraintName("FK_HistoricoChamado_Chamado")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Sugestoes)
                .WithOne(s => s.Chamado)
                .HasForeignKey(s => s.IdChamado)
                .HasConstraintName("FK_SugestaoIA_Chamado")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Anexos)
                .WithOne(a => a.Chamado)
                .HasForeignKey(a => a.IdChamado)
                .HasConstraintName("FK_Anexo_Chamado")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Feedback)
                .WithOne(f => f.Chamado)
                .HasForeignKey<Feedback>(f => f.IdChamado)
                .HasConstraintName("FK_Feedback_Chamado")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureCategoria(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria");
            entity.HasKey(e => e.IdCategoria);
            entity.Property(e => e.IdCategoria).ValueGeneratedOnAdd();
            
            entity.Property(e => e.NomeCategoria)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Descricao)
                .HasMaxLength(255);
        });
    }

    private static void ConfigurePrioridade(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prioridade>(entity =>
        {
            entity.ToTable("Prioridade");
            entity.HasKey(e => e.IdPrioridade);
            entity.Property(e => e.IdPrioridade).ValueGeneratedOnAdd();
            
            entity.Property(e => e.NivelPrioridade)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Descricao)
                .HasMaxLength(255);
        });
    }

    private static void ConfigureStatusChamado(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatusChamado>(entity =>
        {
            entity.ToTable("StatusChamado");
            entity.HasKey(e => e.IdStatus);
            entity.Property(e => e.IdStatus).ValueGeneratedOnAdd();
            
            entity.Property(e => e.NomeStatus)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Descricao)
                .HasMaxLength(255);
        });
    }

    private static void ConfigureHistoricoChamado(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistoricoChamado>(entity =>
        {
            entity.ToTable("HistoricoChamado");
            entity.HasKey(e => e.IdHistorico);
            entity.Property(e => e.IdHistorico).ValueGeneratedOnAdd();
            
            entity.Property(e => e.StatusAnterior)
                .HasMaxLength(50);
            
            entity.Property(e => e.StatusAtual)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.DataHoraAlteracao)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            
            entity.Property(e => e.Observacao)
                .HasMaxLength(255);
        });
    }

    private static void ConfigureSugestaoIA(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SugestaoIA>(entity =>
        {
            entity.ToTable("SugestaoIA");
            entity.HasKey(e => e.IdSugestao);
            entity.Property(e => e.IdSugestao).ValueGeneratedOnAdd();
            
            entity.Property(e => e.TextoSugestao)
                .IsRequired()
                .HasMaxLength(1000);
            
            entity.Property(e => e.DataHoraGeracao)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            
            entity.Property(e => e.FoiAceita)
                .IsRequired()
                .HasDefaultValue(false);
        });
    }

    private static void ConfigureFeedback(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("Feedback", t => t.HasCheckConstraint("CK_Feedback_Nota", "[Nota] BETWEEN 1 AND 5"));
            entity.HasKey(e => e.IdFeedback);
            entity.Property(e => e.IdFeedback).ValueGeneratedOnAdd();
            
            entity.Property(e => e.Nota)
                .IsRequired();
            
            entity.Property(e => e.Comentario)
                .HasMaxLength(500);
            
            entity.Property(e => e.DataHoraFeedback)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            
            // Constraint de uniqueness para garantir apenas um feedback por chamado
            entity.HasIndex(e => e.IdChamado)
                .IsUnique()
                .HasDatabaseName("UC_Feedback_Chamado");
        });
    }

    private static void ConfigureAnexo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anexo>(entity =>
        {
            entity.ToTable("Anexo");
            entity.HasKey(e => e.IdAnexo);
            entity.Property(e => e.IdAnexo).ValueGeneratedOnAdd();
            
            entity.Property(e => e.NomeArquivo)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.CaminhoArquivo)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.DataUpload)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
        });
    }

    private static void ConfigureComentarioChamado(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComentarioChamado>(entity =>
        {
            entity.ToTable("ComentarioChamado");
            entity.HasKey(e => e.IdComentario);
            entity.Property(e => e.IdComentario).ValueGeneratedOnAdd();

            entity.Property(e => e.Mensagem)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.DataHoraEnvio)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.Lido)
                .IsRequired()
                .HasDefaultValue(false);

            // Relacionamento com Chamado (cascade delete - deletar comentários quando chamado for deletado)
            entity.HasOne(e => e.Chamado)
                .WithMany(c => c.Comentarios)
                .HasForeignKey(e => e.IdChamado)
                .HasConstraintName("FK_ComentarioChamado_Chamado")
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com Usuario (restrict - não permitir deletar usuário com comentários)
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.IdUsuario)
                .HasConstraintName("FK_ComentarioChamado_Usuario")
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para performance
            entity.HasIndex(e => e.IdChamado)
                .HasDatabaseName("IX_ComentarioChamado_IdChamado");

            entity.HasIndex(e => new { e.IdChamado, e.DataHoraEnvio })
                .HasDatabaseName("IX_ComentarioChamado_Chamado_Data");

            entity.HasIndex(e => new { e.IdUsuario, e.Lido })
                .HasDatabaseName("IX_ComentarioChamado_Usuario_Lido");
        });
    }
}