using Microsoft.AspNetCore.Mvc;
using Desafio_API.Data;
using System;
using System.Linq;
using Desafio_API.Models;


namespace Desafio_API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class PopularController : ControllerBase
    {

    private readonly ApplicationDbContext database;

     public PopularController(ApplicationDbContext database){
            this.database = database;}


     [HttpPost]   
        public IActionResult PopularBD (){

            try{

            Cliente c1 = new Cliente {Id = 1, Nome = "Antonio Carlos", Email="antonio@teste.com.br", Senha=CalculaHash("123456"), Documento="123.456.789-65", DataCadastro=DateTime.Now};
            Cliente c2 = new Cliente {Id = 2,Nome = "Adriana Maria", Email="adriana@teste.com.br", Senha=CalculaHash("123456"), Documento="123.654.789-65", DataCadastro=DateTime.Now};
            Cliente c3 = new Cliente {Id = 3,Nome = "Ricardo Contins", Email="ricardo@teste.com.br", Senha=CalculaHash("123456"), Documento="223.456.789-85", DataCadastro=DateTime.Now};
            Cliente c4 = new Cliente {Id = 4,Nome = "Salete Burgers", Email="salete@teste.com.br", Senha=CalculaHash("123456"), Documento="453.456.789-65", DataCadastro=DateTime.Now}; 
            Cliente c5 = new Cliente {Id = 5,Nome = "Victor Sanches", Email="victor@teste.com.br", Senha=CalculaHash("123456"), Documento="453.496.459-65", DataCadastro=DateTime.Now};       
            database.Add(c1);
            database.Add(c2);
            database.Add(c3);
            database.Add(c4);
            database.Add(c5);
            database.SaveChanges();


            Fornecedor f1 = new Fornecedor{Id = 1,Nome = "Amazon", CNPJ = "01.456.369/0258-10"};
            Fornecedor f2 =  new Fornecedor{Id = 2,Nome = "Microsoft", CNPJ = "01.456.369/4587-10"};
            Fornecedor f3 =  new Fornecedor{Id = 3,Nome = "Magazine Luiza", CNPJ = "01.456.369/1263-10"};
            Fornecedor f4 =  new Fornecedor{Id = 4,Nome = "Dell Computers", CNPJ = "01.456.369/1478-10"};
            Fornecedor f5 =  new Fornecedor{Id = 5,Nome = "Casas Bahia", CNPJ = "01.456.379/1741-10"};
            database.Add(f1);
            database.Add(f2);
            database.Add(f3);
            database.Add(f4);
            database.Add(f5);
            database.SaveChanges();

            
            Produto p1 = new Produto{Id = 1, Nome = "Echo 3 geração", Codigo = "8005261", Valor= 399.00, Promocao = true, ValorPromocao = 199.00, Categoria = "Eletrônicos", Imagem = "echo.jpeg", Quantidade=10, Fornecedor = database.Fornecedores.First(f=> f.Id == 1)};
            Produto p2 = new Produto{Id = 2, Nome = "Notebook", Codigo = "8004585", Valor= 1500.00, Promocao = false, ValorPromocao = 0, Categoria = "Computadores", Imagem = "notebook.jpeg", Quantidade=5, Fornecedor = database.Fornecedores.First(f=> f.Id == 3)};
            Produto p3 = new Produto{Id = 3, Nome = "X Box 360", Codigo = "8009685", Valor= 900.00, Promocao = false, ValorPromocao = 0, Categoria = "Video Games", Imagem = "xbox.jpeg", Quantidade=20, Fornecedor = database.Fornecedores.First(f=> f.Id == 2)};
            Produto p4 = new Produto{Id = 4, Nome = "Camera Notebook", Codigo = "8004574", Valor= 150.00, Promocao = true, ValorPromocao = 99.00, Categoria = "Eletrônicos", Imagem = "camera.jpeg", Quantidade=50, Fornecedor = database.Fornecedores.First(f=> f.Id == 3)};
            Produto p5 = new Produto{Id = 5, Nome = "Celular Zenphone Azus", Codigo = "8004591", Valor= 1500.00, Promocao = true, ValorPromocao = 1299.00, Categoria = "Eletrônicos", Imagem = "celular.jpeg", Quantidade=25, Fornecedor = database.Fornecedores.First(f=> f.Id == 1)};
            database.Add(p1);
            database.Add(p2);
            database.Add(p3);
            database.Add(p4);
            database.Add(p5);
            database.SaveChanges();

            Venda v1 = new Venda{Id = 1, Cliente = database.Clientes.First(c=> c.Id == 1), DataCompra = DateTime.Now, TotalCompra = 1699.00};
            database.Add(v1);
            database.SaveChanges();
            VendaProduto vp1 = new VendaProduto {
                VendaId = 1,
                ProdutoId= 1,
            };
            VendaProduto vp2 = new VendaProduto {
                VendaId = 1,
                ProdutoId= 2,
            };
            database.AddRange(vp1, vp2);
            database.SaveChanges();

            VendaFornecedor vf1 = new VendaFornecedor{
                VendaId = 1,
                ProdutoId = 1,
                FornecedorId = 1
            };
              VendaFornecedor vf2 = new VendaFornecedor{
                VendaId = 1,
                ProdutoId = 2,
                FornecedorId = 3
            };
            database.AddRange(vf1, vf2);
            database.SaveChanges();


            Venda v2 = new Venda{Id = 2, Cliente = database.Clientes.First(c=> c.Id == 2), DataCompra = DateTime.Now, TotalCompra = 2400.00};
            database.Add(v2);
            database.SaveChanges();
            VendaProduto vp3 = new VendaProduto {
                VendaId = 2,
                ProdutoId= 3,
            };
            VendaProduto vp4 = new VendaProduto {
                VendaId = 2,
                ProdutoId= 2,
            };
            database.AddRange(vp3, vp4);
            database.SaveChanges();

            VendaFornecedor vf3 = new VendaFornecedor{
                VendaId = 2,
                ProdutoId = 3,
                FornecedorId = 2
            };
              VendaFornecedor vf4 = new VendaFornecedor{
                VendaId = 2,
                ProdutoId = 2,
                FornecedorId = 3
            };
            database.AddRange(vf3, vf4);
            database.SaveChanges();








            return Ok ("Dados salvos com sucesso!");

            }catch{

            Response.StatusCode = 405;
            return new ObjectResult (new{msg = "Banco de dados já atualizado, não está mais disponível esse Post!" });

            }
        }

         public static string CalculaHash(string Senha)
    {
        try
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Senha);
            byte[] hash = md5.ComputeHash(inputBytes);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString(); // Retorna senha criptografada 
        }
        catch (Exception)
        {
            return null; // Caso encontre erro retorna nulo
        }
    }



        
    }
}