using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIM.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddComentariosChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComentarioChamado",
                columns: table => new
                {
                    IdComentario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChamado = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DataHoraEnvio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Lido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentarioChamado");
        }
    }
}
