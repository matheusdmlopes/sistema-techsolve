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
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCategoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "PerfilAcesso",
                columns: table => new
                {
                    IdPerfil = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomePerfil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilAcesso", x => x.IdPerfil);
                });

            migrationBuilder.CreateTable(
                name: "Prioridade",
                columns: table => new
                {
                    IdPrioridade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NivelPrioridade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prioridade", x => x.IdPrioridade);
                });

            migrationBuilder.CreateTable(
                name: "Setor",
                columns: table => new
                {
                    IdSetor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeSetor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setor", x => x.IdSetor);
                });

            migrationBuilder.CreateTable(
                name: "StatusChamado",
                columns: table => new
                {
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusChamado", x => x.IdStatus);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrimeiroAcesso = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IdSetor = table.Column<int>(type: "int", nullable: false),
                    IdPerfil = table.Column<int>(type: "int", nullable: false),
                    PerfilAcessoIdPerfil = table.Column<int>(type: "int", nullable: true)
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
                    IdChamado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "VARCHAR(MAX)", maxLength: 2147483647, nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SolucaoFinal = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    IdUsuarioCriador = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioTecnico = table.Column<int>(type: "int", nullable: true),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    IdPrioridade = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false)
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
                    IdAnexo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChamado = table.Column<int>(type: "int", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
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
                name: "Feedback",
                columns: table => new
                {
                    IdFeedback = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChamado = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataHoraFeedback = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
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
                    IdHistorico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChamado = table.Column<int>(type: "int", nullable: false),
                    IdTecnico = table.Column<int>(type: "int", nullable: true),
                    StatusAnterior = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StatusAtual = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataHoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Observacao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
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
                    IdSugestao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChamado = table.Column<int>(type: "int", nullable: false),
                    TextoSugestao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DataHoraGeracao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FoiAceita = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
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
