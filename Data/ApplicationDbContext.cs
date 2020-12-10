
using Microsoft.EntityFrameworkCore;
using Desafio_API.Models;


namespace Desafio_API.Data
{
    public class ApplicationDbContext : DbContext
    {

       private readonly ApplicationDbContext database;

       
        public DbSet<Cliente> Clientes {get; set;} 
        public DbSet<Fornecedor> Fornecedores {get; set;} 
        public DbSet<Produto> Produtos {get; set;} 
        public DbSet<Venda> Vendas {get; set;} 
        public DbSet<VendaProduto> VendasProdutos {get; set;} 
        public DbSet<VendaFornecedor> VendaFornecedores {get; set;} 

         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        { }

         protected override void OnModelCreating(ModelBuilder builder)
         {builder.Entity<VendaProduto>().HasKey(sc => new{sc.ProdutoId, sc.VendaId});
          builder.Entity<VendaFornecedor>().HasKey(sc => new{sc.ProdutoId, sc.VendaId, sc.FornecedorId});

           

         }
        
     }
}
