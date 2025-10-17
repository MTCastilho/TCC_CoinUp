using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coin_up.Migrations
{
    /// <inheritdoc />
    public partial class AddDescricaoTransacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Transacoes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Transacoes");
        }
    }
}
