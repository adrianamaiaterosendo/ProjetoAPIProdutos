using Microsoft.EntityFrameworkCore.Migrations;

namespace Desafio_API.Migrations
{
    public partial class AddVendaFornecedor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendaFornecedores",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(nullable: false),
                    VendaId = table.Column<int>(nullable: false),
                    FornecedorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendaFornecedores", x => new { x.ProdutoId, x.VendaId, x.FornecedorId });
                    table.ForeignKey(
                        name: "FK_VendaFornecedores_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendaFornecedores_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendaFornecedores_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendaFornecedores_FornecedorId",
                table: "VendaFornecedores",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendaFornecedores_VendaId",
                table: "VendaFornecedores",
                column: "VendaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendaFornecedores");
        }
    }
}
