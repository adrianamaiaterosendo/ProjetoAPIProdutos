namespace Desafio_API.Models
{
    public class VendaFornecedor
    {
        public int ProdutoId{get; set;}
        public Produto Produtos{get; set;}
        public int VendaId{get; set;}
        public Venda Vendas{get; set;}
        public int FornecedorId{get; set;}
        public Fornecedor Fornecedores {get; set;}
        
    }
}