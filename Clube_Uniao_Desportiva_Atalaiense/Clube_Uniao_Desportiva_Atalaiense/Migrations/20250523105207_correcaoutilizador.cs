using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net.Mail;

#nullable disable

namespace Clube_Uniao_Desportiva_Atalaiense.Migrations
{
    /// <inheritdoc />
    public partial class correcaoutilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_Utilizadores_UtilizadorUsername",
                table: "Favoritos");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.RenameColumn(
                name: "UtilizadorUsername",
                table: "Favoritos",
                newName: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_AspNetUsers_UtilizadorId",
                table: "Favoritos",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_AspNetUsers_UtilizadorId",
                table: "Favoritos");

            migrationBuilder.RenameColumn(
                name: "UtilizadorId",
                table: "Favoritos",
                newName: "UtilizadorUsername");

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.Username);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_Utilizadores_UtilizadorUsername",
                table: "Favoritos",
                column: "UtilizadorUsername",
                principalTable: "Utilizadores",
            principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}




