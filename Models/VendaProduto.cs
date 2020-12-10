
namespace Desafio_API.Models
{
    public class VendaProduto
    {
        public int ProdutoId{get; set;}

        public Produto Produtos{get; set;}

        public int VendaId{get; set;}
        public Venda Vendas{get; set;}
    }
}