using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Desafio_API.Models;
using System.Linq;


namespace Desafio_API.Data
{
    public class ApplicationDbContext : DbContext
    {

       // private readonly ApplicationDbContext database;

       
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

        //     builder.Entity<Cliente>()
        //     .HasData(new Cliente {Nome = "Antonio Carlos", Email="antonio@teste.com.br", Senha="123456", Documento="123.456.789-65", DataCadastro=DateTime.Now},
        //              new Cliente {Nome = "Adriana Maria", Email="adriana@teste.com.br", Senha="123456", Documento="123.654.789-65", DataCadastro=DateTime.Now},
        //              new Cliente {Nome = "Ricardo Contins", Email="ricardo@teste.com.br", Senha="123456", Documento="223.456.789-85", DataCadastro=DateTime.Now},
        //              new Cliente {Nome = "Salete Burgers", Email="salete@teste.com.br", Senha="123456", Documento="453.456.789-65", DataCadastro=DateTime.Now}           

        //     );

        //     builder.Entity<Fornecedor>()
        //     .HasData( new Fornecedor{Nome = "Amazon", CNPJ = "01.456.369/0258-10"},
        //               new Fornecedor{Nome = "Microsoft", CNPJ = "01.456.369/4587-10"},
        //               new Fornecedor{Nome = "Magazine Luiza", CNPJ = "01.456.369/1263-10"}

        //     );

        //     builder.Entity<Produto>()
        //     .HasData( new Produto{Nome = "Echo 3 geração", Codigo = "8005261", Valor= 399.00, Promocao = true, ValorPromocao = 199.00, Categoria = "Eletrônicos", Imagem = "echo.jpeg", Quantidade=10, Fornecedor = database.Fornecedores.First(f=> f.Id == 1)},
        //               new Produto{Nome = "Notebook", Codigo = "8004585", Valor= 1500.00, Promocao = false, ValorPromocao = 0, Categoria = "Computadores", Imagem = "notebook.jpeg", Quantidade=5, Fornecedor = database.Fornecedores.First(f=> f.Id == 3)},
        //               new Produto{Nome = "X Box 360", Codigo = "8009685", Valor= 900.00, Promocao = false, ValorPromocao = 0, Categoria = "Video Games", Imagem = "xbox.jpeg", Quantidade=20, Fornecedor = database.Fornecedores.First(f=> f.Id == 2)},
        //               new Produto{Nome = "Camera Notebook", Codigo = "8004574", Valor= 150.00, Promocao = true, ValorPromocao = 99.00, Categoria = "Eletrônicos", Imagem = "camera.jpeg", Quantidade=50, Fornecedor = database.Fornecedores.First(f=> f.Id == 3)},
        //               new Produto{Nome = "Celular Zenphone Azus", Codigo = "8004591", Valor= 1500.00, Promocao = true, ValorPromocao = 1299.00, Categoria = "Eletrônicos", Imagem = "celular.jpeg", Quantidade=25, Fornecedor = database.Fornecedores.First(f=> f.Id == 1)}  
        //     ); 


         }
        
     }
}
