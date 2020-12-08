using Microsoft.AspNetCore.Mvc;
using Desafio_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Desafio_API.Models;
using Desafio_API.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Newtonsoft.Json;
using Desafio_API.HATEOAS;

namespace Desafio_API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {  

        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public ProdutosController(ApplicationDbContext database){
            this.database = database;
             
            HATEOAS = new HATEOAS.HATEOAS ("localhost:5001/api/v1/produtos");
            HATEOAS.AddAction("GET_LIST", "GET");
            HATEOAS.AddAction("DELETE_PRODUTOS", "DELETE");
            HATEOAS.AddAction("EDIT_PRODUTOS", "PATCH");
            
        }
        [HttpGet]   
        public IActionResult ListaProdutos (){
            var produtos = database.Produtos.Include(f=> f.Fornecedor).ToList();

            List<ProdutoGetContainer> produtosHATEOAS = new List<ProdutoGetContainer>();
            foreach(var produto in produtos){
            ProdutoGetContainer produtoHateoas = new ProdutoGetContainer();
            produtoHateoas.produtos = produto;
            produtoHateoas.links = HATEOAS.GetActions(produto.Id.ToString());
            produtosHATEOAS.Add(produtoHateoas);}
             
           return Ok(new{produtosHATEOAS}); 
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoDTO pDTO){
            Produto produto = new Produto();

              if(pDTO.Nome.Length <= 4){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um nome válido, com pelo menos 5 caracteres!"});
                }
              if (pDTO.Codigo.Length < 6){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um código válido, com pelo menos 6 caracteres!"});
                }
              if(pDTO.Valor <= 0){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um valor válido, valor não pode ser negativo ou zerado!"});
                } 
             
                  if(pDTO.Promocao == true){
                      if(pDTO.ValorPromocao > 0){

                        produto.Promocao = pDTO.Promocao;
                        produto.ValorPromocao = pDTO.ValorPromocao;
                      }else{
                          Response.StatusCode = 400;
                          return new ObjectResult (new{msg="Favor inserir um valor para sua promoção!"});

                      }
                  }
                  if(pDTO.Promocao == false){
                      if(pDTO.ValorPromocao == 0){

                        produto.Promocao = pDTO.Promocao;
                        produto.ValorPromocao = pDTO.ValorPromocao;       

                      }else{
                          Response.StatusCode = 400;
                          return new ObjectResult (new{msg="Favor inserir o valor zero no campo Valor Promoção!"});
                        }
                    }
               
             
                
              if(pDTO.Imagem.Length <= 2){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir uma imagem"});
                }

              if(pDTO.Categoria.Length <= 2){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir uma categoria com mais de 2 caracteres"});
                }

              if(pDTO.Quantidade <= 0){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="A quantidade de produtos não pode ser menos que 0!"});
                }

            
              try{

                 if(pDTO.FornecedorId <= 0){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor informar um fornecedor para o produto"});
                }
              

                produto.Nome =  pDTO.Nome;
                produto.Codigo = pDTO.Codigo;
                produto.Valor = pDTO.Valor;
                
                produto.Categoria = pDTO.Categoria;
                produto.Imagem = pDTO.Imagem;
                produto.Quantidade = pDTO.Quantidade;
                produto.Fornecedor = database.Fornecedores.First(f=> f.Id == pDTO.FornecedorId);
                

            
                database.Produtos.Add(produto);
                database.SaveChanges();    
                          

            
                 Response.StatusCode = 201;
                 return new ObjectResult (new{msg = "Produto cadastrado com sucesso!" });


              }catch{
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Fornecedor não localizado!"});
              }                    
              
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                var produtos = database.Produtos.Include(f=> f.Fornecedor).First(f=> f.Id == id);
                ProdutoGetContainer produtoHATEOAS = new ProdutoGetContainer();
                produtoHATEOAS.produtos = produtos;
                produtoHATEOAS.links = HATEOAS.GetActions(produtos.Id.ToString());
                             
            return Ok(produtoHATEOAS);
               
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        }

        [HttpPut]
        public IActionResult Put([FromBody] ProdutoDTO produto){

             if(produto.Id > 0){
                try{ 

                    var produtoTemp = database.Produtos.First(p => p.Id == produto.Id);
                   
                            

                    if(produtoTemp != null){

                        if(produto.Nome != null ){
                            if(produto.Nome.Length <= 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Nome inválido ou vazio, tente outro nome!"}); 
                            }
                            produtoTemp.Nome = produto.Nome;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Nome = produtoTemp.Nome;
                            database.SaveChanges();
                        }

                           if(produto.Codigo != null ){
                            if(produto.Codigo.Length < 6){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um código válido, com pelo menos 6 caracteres!"}); 
                            }
                            produtoTemp.Codigo = produto.Codigo;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Codigo = produtoTemp.Codigo;
                            database.SaveChanges();
                        }
                        if(produto.Valor != 0){
                            if(produto.Valor <= 0){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um valor válido, maior que 0!"}); 
                            }
                            produtoTemp.Valor = produto.Valor;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Valor = produtoTemp.Valor;
                            database.SaveChanges();
                        }
                        if(produto.Promocao == false || produto.Promocao == true){
                          produtoTemp.Promocao = produto.Promocao;
                          if(produto.Promocao == true && produto.ValorPromocao <= 0){                           
                              Response.StatusCode = 400;
                              return new ObjectResult (new{msg="Favor inserir um valor válido maior que 0 no campo Valor Promoção!"}); 
                            
                          }
                          if(produto.Promocao == false && produto.ValorPromocao > 0){
                              Response.StatusCode = 400;
                              return new ObjectResult (new{msg="O valor da promoção deve ser 0!"}); 
                          }else{
                            produtoTemp.ValorPromocao = produto.ValorPromocao;
                            database.SaveChanges();
                          }

                            if(produto.Promocao != false || produto.Promocao != true){
                                produtoTemp.Promocao = produtoTemp.Promocao;
                                database.SaveChanges();
                            }

                          

                            database.SaveChanges();
                        }   database.SaveChanges();
                        }else{
                            produtoTemp.Valor = produtoTemp.Valor;                    
                               
                            return Ok("Alterações efetuadas com sucesso!");
                            }
                            if(produto.Categoria != null ){
                            if(produto.Categoria.Length < 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor informar uma categoria com mais de 2 caracteres!"}); 
                            }
                            produtoTemp.Categoria = produto.Categoria;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Categoria = produtoTemp.Categoria;
                            database.SaveChanges();
                        }

                          if(produto.Imagem != null ){
                            if(produto.Imagem.Length < 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor informar uma imagem válida formato: 'exemplo.jpeg'!"}); 
                            }
                            produtoTemp.Imagem = produto.Imagem;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Imagem = produtoTemp.Imagem;
                            database.SaveChanges();
                        }
                             if(produto.Quantidade != 0){
                            if(produto.Quantidade <= 0){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um valor válido, maior que 0!"}); 
                            }
                            produtoTemp.Quantidade = produto.Quantidade;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Quantidade = produtoTemp.Quantidade;
                            database.SaveChanges();
                        }

                        if(produto.FornecedorId <= 0){
                          Response.StatusCode = 400;
                          return new ObjectResult (new{msg="Fornecedor Inválido, deve inserir um fornecedor!"}); 
                          
                        }else{

                          if(produto.FornecedorId > 0){
                            try{
                          var fornecedor = database.Fornecedores.First(f=> f.Id == produto.FornecedorId);
                          produtoTemp.Fornecedor = fornecedor;
                          database.SaveChanges();
                             }catch{
                                Response.StatusCode = 404;
                                return new ObjectResult (new{msg="Fornecedor não localizado!"}); 


                             }
                          }                 

                }}catch{

                    Response.StatusCode = 404;
                    return new ObjectResult (new{msg="Produto não localizado"}); 

                }

            }if(produto == null){
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="Produto não encontrado"}); }
            return Ok("Alterações efetuadas com sucesso!");



        } 

        [HttpDelete("{id}")]
        public IActionResult Delete(int id){

               try{
                var produtos = database.Produtos.First(f=> f.Id == id);
                database.Produtos.Remove(produtos);
                database.SaveChanges();
               
            return Ok("Produto excluído com sucesso"); 
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        } 
        
         
         
         [HttpGet("asc")]   
        public IActionResult ListaAlfCres(){
            var produtos = database.Produtos.ToList();

            IEnumerable<Produto> produto = from word in produtos
                            orderby word.Nome
                            select word;  
  
            foreach (var str in produto)  {

            }
             
             
           return Ok(new{produto}); 
        }

         [HttpGet("desc")]   
        public IActionResult ListaAlfDec(){
            var produtos = database.Produtos.ToList();

            IEnumerable<Produto> produto = from word in produtos
                            orderby word.Nome descending  
                            select word;  
  
            foreach (var str in produto)  {

            }
             
             
           return Ok(new{produto}); 
        }

           [HttpGet("nome/{nome}")]   
        public IActionResult PesquisaNome(string nome){
            try{
            var produto= database.Produtos.Where(p=> p.Nome.Contains(nome)).ToList();

             List<ProdutoGetContainer> produtosHATEOAS = new List<ProdutoGetContainer>();
            foreach(var produtos in produto){
                ProdutoGetContainer produtoHateoas = new ProdutoGetContainer();
                produtoHateoas.produtos = produtos;
                produtoHateoas.links = HATEOAS.GetActions(produtos.Id.ToString());
               produtosHATEOAS.Add(produtoHateoas);}

            if(produto.Count == 0){
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de produtos"}); }
               
             
           return Ok(new{produtosHATEOAS}); 
           }catch{
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de produtos"}); }
           
        }


          [HttpPatch]
        public IActionResult Editar ([FromBody] ProdutoDTO produto){           
           
            if(produto.Id > 0){
                try{ 

                    var produtoTemp = database.Produtos.First(p => p.Id == produto.Id);
                   
                            

                    if(produtoTemp != null){

                        if(produto.Nome != null ){
                            if(produto.Nome.Length <= 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Nome inválido ou vazio, tente outro nome!"}); 
                            }
                            produtoTemp.Nome = produto.Nome;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Nome = produtoTemp.Nome;
                            database.SaveChanges();
                        }

                           if(produto.Codigo != null ){
                            if(produto.Codigo.Length < 6){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um código válido, com pelo menos 6 caracteres!"}); 
                            }
                            produtoTemp.Codigo = produto.Codigo;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Codigo = produtoTemp.Codigo;
                            database.SaveChanges();
                        }
                        if(produto.Valor != 0){
                            if(produto.Valor <= 0){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um valor válido, maior que 0!"}); 
                            }
                            produtoTemp.Valor = produto.Valor;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Valor = produtoTemp.Valor;
                            database.SaveChanges();
                        }
                        if(produto.Promocao == false || produto.Promocao == true){
                          produtoTemp.Promocao = produto.Promocao;
                          if(produto.Promocao == true && produto.ValorPromocao <= 0){                           
                              Response.StatusCode = 400;
                              return new ObjectResult (new{msg="Favor inserir um valor válido maior que 0 no campo Valor Promoção!"}); 
                            
                          }
                          if(produto.Promocao == false && produto.ValorPromocao > 0){
                              Response.StatusCode = 400;
                              return new ObjectResult (new{msg="O valor da promoção deve ser 0!"}); 
                          }else{
                            produtoTemp.ValorPromocao = produto.ValorPromocao;
                            database.SaveChanges();
                          }

                            if(produto.Promocao != false || produto.Promocao != true){
                                produtoTemp.Promocao = produtoTemp.Promocao;
                                database.SaveChanges();
                            }

                          

                            database.SaveChanges();
                        }   database.SaveChanges();
                        }else{
                            produtoTemp.Valor = produtoTemp.Valor;                    
                               
                            return Ok("Alterações efetuadas com sucesso!");
                            }
                            if(produto.Categoria != null ){
                            if(produto.Categoria.Length < 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor informar uma categoria com mais de 2 caracteres!"}); 
                            }
                            produtoTemp.Categoria = produto.Categoria;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Categoria = produtoTemp.Categoria;
                            database.SaveChanges();
                        }

                          if(produto.Imagem != null ){
                            if(produto.Imagem.Length < 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor informar uma imagem válida formato: 'exemplo.jpeg'!"}); 
                            }
                            produtoTemp.Imagem = produto.Imagem;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Imagem = produtoTemp.Imagem;
                            database.SaveChanges();
                        }
                             if(produto.Quantidade != 0){
                            if(produto.Quantidade <= 0){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um valor válido, maior que 0!"}); 
                            }
                            produtoTemp.Quantidade = produto.Quantidade;
                            database.SaveChanges();
                        }else{
                            produtoTemp.Quantidade = produtoTemp.Quantidade;
                            database.SaveChanges();
                        }

                        if(produto.FornecedorId <= 0){
                          Response.StatusCode = 400;
                          return new ObjectResult (new{msg="Fornecedor Inválido!"}); 
                          
                        }else{

                          if(produto.FornecedorId > 0){
                            try{
                          var fornecedor = database.Fornecedores.First(f=> f.Id == produto.FornecedorId);
                          produtoTemp.Fornecedor = fornecedor;
                          database.SaveChanges();
                             }catch{
                                Response.StatusCode = 404;
                                return new ObjectResult (new{msg="Fornecedor não localizado!"}); 


                             }
                          }                 

                }}catch{

                    Response.StatusCode = 404;
                    return new ObjectResult (new{msg="Produto não localizado"}); 

                }

            }if(produto == null){
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="oi Produto não encontrado"}); }
            return Ok("Alterações efetuadas com sucesso!");
           
        }


         public class ProdutoGetContainer{
            public Produto produtos {get; set;}

            public Link[] links {get; set;}
        }

    }
        
}
