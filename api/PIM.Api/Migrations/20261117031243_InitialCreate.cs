using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIM.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeCategoria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "PerfilAcesso",
                columns: table => new
                {
                    IdPerfil = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomePerfil = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilAcesso", x => x.IdPerfil);
                });

            migrationBuilder.CreateTable(
                name: "Prioridade",
                columns: table => new
                {
                    IdPrioridade = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NivelPrioridade = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prioridade", x => x.IdPrioridade);
                });

            migrationBuilder.CreateTable(
                name: "Setor",
                columns: table => new
                {
                    IdSetor = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeSetor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setor", x => x.IdSetor);
                });

            migrationBuilder.CreateTable(
                name: "StatusChamado",
                columns: table => new
                {
                    IdStatus = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusChamado", x => x.IdStatus);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    UltimoLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PrimeiroAcesso = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IdSetor = table.Column<int>(type: "INTEGER", nullable: false),
                    IdPerfil = table.Column<int>(type: "INTEGER", nullable: false),
                    PerfilAcessoIdPerfil = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_PerfilAcesso",
                        column: x => x.IdPerfil,
                        principalTable: "PerfilAcesso",
                        principalColumn: "IdPerfil",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_PerfilAcesso_PerfilAcessoIdPerfil",
                        column: x => x.PerfilAcessoIdPerfil,
                        principalTable: "PerfilAcesso",
                        principalColumn: "IdPerfil");
                    table.ForeignKey(
                        name: "FK_Usuario_Setor",
                        column: x => x.IdSetor,
                        principalTable: "Setor",
                        principalColumn: "IdSetor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chamado",
                columns: table => new
                {
                    IdChamado = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    DataFechamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SolucaoFinal = table.Column<string>(type: "TEXT", nullable: true),
                    IdUsuarioCriador = table.Column<int>(type: "INTEGER", nullable: false),
                    IdUsuarioTecnico = table.Column<int>(type: "INTEGER", nullable: true),
                    IdCategoria = table.Column<int>(type: "INTEGER", nullable: false),
                    IdPrioridade = table.Column<int>(type: "INTEGER", nullable: false),
                    IdStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chamado", x => x.IdChamado);
                    table.ForeignKey(
                        name: "FK_Chamado_Categoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categoria",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chamado_Prioridade",
                        column: x => x.IdPrioridade,
                        principalTable: "Prioridade",
                        principalColumn: "IdPrioridade",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chamado_Status",
                        column: x => x.IdStatus,
                        principalTable: "StatusChamado",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chamado_UsuarioCriador",
                        column: x => x.IdUsuarioCriador,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chamado_UsuarioTecnico",
                        column: x => x.IdUsuarioTecnico,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Anexo",
                columns: table => new
                {
                    IdAnexo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdChamado = table.Column<int>(type: "INTEGER", nullable: false),
                    NomeArquivo = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DataUpload = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexo", x => x.IdAnexo);
                    table.ForeignKey(
                        name: "FK_Anexo_Chamado",
                        column: x => x.IdChamado,
                        principalTable: "Chamado",
                        principalColumn: "IdChamado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComentarioChamado",
                columns: table => new
                {
                    IdComentario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdChamado = table.Column<int>(type: "INTEGER", nullable: false),
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DataHoraEnvio = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    Lido = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentarioChamado", x => x.IdComentario);
                    table.ForeignKey(
                        name: "FK_ComentarioChamado_Chamado",
                        column: x => x.IdChamado,
                        principalTable: "Chamado",
                        principalColumn: "IdChamado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentarioChamado_Usuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    IdFeedback = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdChamado = table.Column<int>(type: "INTEGER", nullable: false),
                    Nota = table.Column<int>(type: "INTEGER", nullable: false),
                    Comentario = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataHoraFeedback = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.IdFeedback);
                    table.CheckConstraint("CK_Feedback_Nota", "[Nota] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Feedback_Chamado",
                        column: x => x.IdChamado,
                        principalTable: "Chamado",
                        principalColumn: "IdChamado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoChamado",
                columns: table => new
                {
                    IdHistorico = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdChamado = table.Column<int>(type: "INTEGER", nullable: false),
                    IdTecnico = table.Column<int>(type: "INTEGER", nullable: true),
                    StatusAnterior = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    StatusAtual = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataHoraAlteracao = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoChamado", x => x.IdHistorico);
                    table.ForeignKey(
                        name: "FK_HistoricoChamado_Chamado",
                        column: x => x.IdChamado,
                        principalTable: "Chamado",
                        principalColumn: "IdChamado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricoChamado_Tecnico",
                        column: x => x.IdTecnico,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SugestaoIA",
                columns: table => new
                {
                    IdSugestao = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdChamado = table.Column<int>(type: "INTEGER", nullable: false),
                    TextoSugestao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DataHoraGeracao = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    FoiAceita = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SugestaoIA", x => x.IdSugestao);
                    table.ForeignKey(
                        name: "FK_SugestaoIA_Chamado",
                        column: x => x.IdChamado,
                        principalTable: "Chamado",
                        principalColumn: "IdChamado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anexo_IdChamado",
                table: "Anexo",
                column: "IdChamado");

            migrationBuilder.CreateIndex(
                name: "IX_Chamado_IdCategoria",
                table: "Chamado",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Chamado_IdPrioridade",
                table: "Chamado",
                column: "IdPrioridade");

            migrationBuilder.CreateIndex(
                name: "IX_Chamado_IdStatus",
                table: "Chamado",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Chamado_IdUsuarioCriador",
                table: "Chamado",
                column: "IdUsuarioCriador");

            migrationBuilder.CreateIndex(
                name: "IX_Chamado_IdUsuarioTecnico",
                table: "Chamado",
                column: "IdUsuarioTecnico");

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioChamado_Chamado_Data",
                table: "ComentarioChamado",
                columns: new[] { "IdChamado", "DataHoraEnvio" });

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioChamado_IdChamado",
                table: "ComentarioChamado",
                column: "IdChamado");

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioChamado_Usuario_Lido",
                table: "ComentarioChamado",
                columns: new[] { "IdUsuario", "Lido" });

            migrationBuilder.CreateIndex(
                name: "UC_Feedback_Chamado",
                table: "Feedback",
                column: "IdChamado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoChamado_IdChamado",
                table: "HistoricoChamado",
                column: "IdChamado");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoChamado_IdTecnico",
                table: "HistoricoChamado",
                column: "IdTecnico");

            migrationBuilder.CreateIndex(
                name: "IX_SugestaoIA_IdChamado",
                table: "SugestaoIA",
                column: "IdChamado");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdPerfil",
                table: "Usuario",
                column: "IdPerfil");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdSetor",
                table: "Usuario",
                column: "IdSetor");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_PerfilAcessoIdPerfil",
                table: "Usuario",
                column: "PerfilAcessoIdPerfil");

            migrationBuilder.CreateIndex(
                name: "UC_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anexo");

            migrationBuilder.DropTable(
                name: "ComentarioChamado");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "HistoricoChamado");

            migrationBuilder.DropTable(
                name: "SugestaoIA");

            migrationBuilder.DropTable(
                name: "Chamado");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Prioridade");

            migrationBuilder.DropTable(
                name: "StatusChamado");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "PerfilAcesso");

            migrationBuilder.DropTable(
                name: "Setor");
        }
    }
}
