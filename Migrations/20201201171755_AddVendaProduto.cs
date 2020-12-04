using Microsoft.EntityFrameworkCore.Migrations;

namespace Desafio_API.Migrations
{
    public partial class AddVendaProduto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendasProdutos",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(nullable: false),
                    VendaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendasProdutos", x => new { x.ProdutoId, x.VendaId });
                    table.ForeignKey(
                        name: "FK_VendasProdutos_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendasProdutos_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendasProdutos_VendaId",
                table: "VendasProdutos",
                column: "VendaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendasProdutos");
        }
    }
}
