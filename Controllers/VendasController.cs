using Microsoft.AspNetCore.Mvc;
using Desafio_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Desafio_API.Models;
using Desafio_API.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Desafio_API.HATEOAS;
using Newtonsoft.Json;


namespace Desafio_API.Controllers
{  
    
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]    
    public class VendasController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        private HATEOAS.HATEOAS HATEOAS;

        public VendasController(ApplicationDbContext database){
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS ("localhost:5001/api/v1/vendas");
            HATEOAS.AddAction("GET_LIST", "GET");
            HATEOAS.AddAction("DELETE_VENDAS", "DELETE");
            HATEOAS.AddAction("EDIT_VENDAS", "PATCH");
            
        }
        
        [HttpGet]   
        public IActionResult ListaVendas (){
            var vendas = database.Vendas.Include(p=> p.VendaProdutos).Include(f=> f.Fornecedor).Include(c=> c.Cliente).ToList();
             List<VendasGetContainer> vendasHATEOAS = new List<VendasGetContainer>();
            foreach(var venda in vendas){
            VendasGetContainer vendaHateoas = new VendasGetContainer();
            vendaHateoas.Id = venda.Id;
            vendaHateoas.ClienteId = venda.Cliente.Id;

            var prodId = database.VendaFornecedores.Where(p=> p.VendaId == venda.Id).ToList();
            
            List<int> idProd = new List<int>();
            List<int> idFunc = new List<int>();
            foreach(var prod in prodId){
                idProd.Add(prod.ProdutoId);
                idFunc.Add(prod.FornecedorId);
            }

           vendaHateoas.ProdutosId = idProd;
           vendaHateoas.FornecedorId = idFunc;

            vendaHateoas.TotalCompra = venda.TotalCompra;
            vendaHateoas.DataCompra = Convert.ToString(venda.DataCompra).Substring(0,10);
            vendaHateoas.links = HATEOAS.GetActions(venda.Id.ToString());
            vendasHATEOAS.Add(vendaHateoas);}
             
             
           return Ok(new{vendasHATEOAS}); 
        }


        [HttpPut]
        public IActionResult Put ([FromBody] VendaDTO venda){  
             

            if(venda.Id >= 0){
                try{
               var vendaLoc = database.Vendas.First(v=> v.Id == venda.Id); 
               if(venda.ClienteId > 0){
                   
                   try{
                   var cliLoc = database.Clientes.First(c=> c.Id == venda.ClienteId);
                   vendaLoc.Cliente = cliLoc;
                   database.SaveChanges();
                   }catch{

                    Response.StatusCode = 400;
                    return new ObjectResult (new{msg="Cliente não localizado!"}); 

                   }

               } else{
                   vendaLoc.Cliente = vendaLoc.Cliente;
                   database.SaveChanges();                   

               }

                if(venda.ProdutosId != null){
                    var prod = database.VendasProdutos.Where(p=> p.VendaId == venda.Id);
                    var forn = database.VendaFornecedores.Where(p=> p.VendaId == venda.Id);
                        database.VendasProdutos.RemoveRange(prod);
                        database.VendaFornecedores.RemoveRange(forn);
                        database.SaveChanges();

                         var VendaProdTemp = database.VendasProdutos.ToList();
                            foreach (var vendaProdId in venda.ProdutosId){
                       
                            VendaProduto vendaProd1 = new VendaProduto();
                            vendaProd1.VendaId = venda.Id;
                            vendaProd1.ProdutoId = vendaProdId;

                            database.VendasProdutos.Add(vendaProd1);                        
                            database.SaveChanges();
                            };



                            foreach(var vendaProdFornId in venda.ProdutosId){
                            VendaFornecedor vendaForn1 = new VendaFornecedor();
                            vendaForn1.VendaId = venda.Id;
                            vendaForn1.ProdutoId = vendaProdFornId;
                            var produto = database.Produtos.Include(f=> f.Fornecedor).First(p=> p.Id == vendaProdFornId);
                            vendaForn1.FornecedorId = produto.Fornecedor.Id;

                            database.VendaFornecedores.Add(vendaForn1);                        
                            database.SaveChanges();
                            };
                
                  double totalCompra = 0;

                
                foreach (var produtoCompra in venda.ProdutosId){
                       
                        var produtoVendas = database.Produtos.First(p=> p.Id == produtoCompra);
                        
                        if(produtoVendas.Promocao == true){

                        totalCompra = produtoVendas.ValorPromocao + totalCompra;

                        }else{
                        totalCompra = produtoVendas.Valor + totalCompra;

                        }

                    };

                venda.TotalCompra = totalCompra;
                vendaLoc.TotalCompra = totalCompra;
                vendaLoc.DataCompra = DateTime.Now;
            
                database.SaveChanges();  



                }else{

                    vendaLoc.VendaProdutos = vendaLoc.VendaProdutos;
                    
                    
                }

                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult (new{msg="Venda não localizada!"}); 


                }

               

            }    
            Response.StatusCode = 200;
            return new ObjectResult (new{msg="Venda alterada com sucesso!"}); 


        }


        [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                var vendas = database.Vendas.Include(p=> p.VendaProdutos).Include(f=> f.Fornecedor).First(f=> f.Id == id);
                VendasGetContainer vendaHATEOAS = new VendasGetContainer();
                vendaHATEOAS.vendas = vendas;
                vendaHATEOAS.links = HATEOAS.GetActions(vendas.Id.ToString());
                             
            return Ok(vendaHATEOAS);
               
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        }

        [HttpGet("asc")]   
        public IActionResult ListaAlfCres(){
            var vendas = database.Vendas.ToList();

            IEnumerable<Venda> venda = from word in vendas
                            orderby word.DataCompra
                            select word;  
  
            foreach (var str in venda)  {

            }
             
             
           return Ok(new{venda}); 
        }

        [HttpGet("desc")]   
        public IActionResult ListaAlfDec(){
            var vendas = database.Vendas.ToList();

            IEnumerable<Venda> venda = from word in vendas 
                            orderby word.DataCompra descending  
                            select word;  
  
            foreach (var str in venda)  {

            }
             
             
           return Ok(new{venda}); 
        }

        [HttpGet("nome/{nome}")]   
        public IActionResult PesquisaNome(string nome){
            try{
            var venda= database.Vendas.Where(v=> v.Cliente.Nome.Contains(nome)).ToList();

             List<VendasGetContainer> vendasHATEOAS = new List<VendasGetContainer>();
            foreach(var vendas in venda){
                VendasGetContainer vendaHateoas = new VendasGetContainer();
                vendaHateoas.vendas = vendas;
                vendaHateoas.links = HATEOAS.GetActions(vendas.Id.ToString());
                vendasHATEOAS.Add(vendaHateoas);}

            if(venda.Count == 0){
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome do cliente não disponível na lista de vendas"}); }
               
             
           return Ok(new{vendasHATEOAS}); 
           }catch{
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome do cliente não disponível na lista de vendas"}); }
           
        }
                       
        [HttpPost]
        public IActionResult Post([FromBody] VendaDTO vDTO){
            Venda venda = new Venda();

             
              if (vDTO.ClienteId <= 0){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Id de cliente inválido!"});
                }                
                try{
                     venda.Cliente = database.Clientes.First(c=> c.Id == vDTO.ClienteId);
                }catch{
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Cliente inexistente!"});

                }

                    vDTO.DataCompra = DateTime.Now;

                   database.Vendas.Add(venda);
                   database.SaveChanges();   

                foreach (var produtosId in vDTO.ProdutosId){
                       
                        VendaProduto vendasProdutos1 = new VendaProduto();
                        vendasProdutos1.ProdutoId = produtosId;
                        vendasProdutos1.VendaId = venda.Id;

                        database.VendasProdutos.Add(vendasProdutos1);                        
                        database.SaveChanges();
                    };

                foreach(var vendaProdFornId in vDTO.ProdutosId){
                            VendaFornecedor vendaForn1 = new VendaFornecedor();
                            vendaForn1.VendaId = venda.Id;
                            vendaForn1.ProdutoId = vendaProdFornId;
                            var produto = database.Produtos.Include(f=> f.Fornecedor).First(p=> p.Id == vendaProdFornId);
                            vendaForn1.FornecedorId = produto.Fornecedor.Id;

                            database.VendaFornecedores.Add(vendaForn1);                        
                            database.SaveChanges();
                            };

                    double totalCompra = 0;

                
                foreach (var produtoCompra in vDTO.ProdutosId){
                       
                        var produtoVendas = database.Produtos.First(p=> p.Id == produtoCompra);
                        
                        if(produtoVendas.Promocao == true){

                        totalCompra = produtoVendas.ValorPromocao + totalCompra;

                        }else{
                        totalCompra = produtoVendas.Valor + totalCompra;

                        }

                    };

                vDTO.TotalCompra = totalCompra;

                venda.TotalCompra = totalCompra;

               
                   
                database.SaveChanges();      




         

           
            Response.StatusCode = 201;
            return new ObjectResult (new{msg = "Venda efetuada com sucesso!" });
        }

        [HttpPatch]
        public IActionResult Editar ([FromBody] VendaDTO venda){  
             

            if(venda.Id >= 0){
                try{
               var vendaLoc = database.Vendas.First(v=> v.Id == venda.Id); 
               if(venda.ClienteId > 0){
                   
                   try{
                   var cliLoc = database.Clientes.First(c=> c.Id == venda.ClienteId);
                   vendaLoc.Cliente = cliLoc;
                   database.SaveChanges();
                   }catch{

                    Response.StatusCode = 400;
                    return new ObjectResult (new{msg="Cliente não localizado!"}); 

                   }

               } else{
                   vendaLoc.Cliente = vendaLoc.Cliente;
                   database.SaveChanges();                   

               }

                if(venda.ProdutosId != null){
                    var prod = database.VendasProdutos.Where(p=> p.VendaId == venda.Id);
                    var forn = database.VendaFornecedores.Where(p=> p.VendaId == venda.Id);
                        database.VendasProdutos.RemoveRange(prod);
                        database.VendaFornecedores.RemoveRange(forn);
                        database.SaveChanges();

                         var VendaProdTemp = database.VendasProdutos.ToList();
                            foreach (var vendaProdId in venda.ProdutosId){
                       
                            VendaProduto vendaProd1 = new VendaProduto();
                            vendaProd1.VendaId = venda.Id;
                            vendaProd1.ProdutoId = vendaProdId;

                            database.VendasProdutos.Add(vendaProd1);                        
                            database.SaveChanges();
                            };



                            foreach(var vendaProdFornId in venda.ProdutosId){
                            VendaFornecedor vendaForn1 = new VendaFornecedor();
                            vendaForn1.VendaId = venda.Id;
                            vendaForn1.ProdutoId = vendaProdFornId;
                            var produto = database.Produtos.Include(f=> f.Fornecedor).First(p=> p.Id == vendaProdFornId);
                            vendaForn1.FornecedorId = produto.Fornecedor.Id;

                            database.VendaFornecedores.Add(vendaForn1);                        
                            database.SaveChanges();
                            };
                
                  double totalCompra = 0;

                
                foreach (var produtoCompra in venda.ProdutosId){
                       
                        var produtoVendas = database.Produtos.First(p=> p.Id == produtoCompra);
                        
                        if(produtoVendas.Promocao == true){

                        totalCompra = produtoVendas.ValorPromocao + totalCompra;

                        }else{
                        totalCompra = produtoVendas.Valor + totalCompra;

                        }

                    };

                venda.TotalCompra = totalCompra;
                vendaLoc.TotalCompra = totalCompra;
                vendaLoc.DataCompra = DateTime.Now;
            
                database.SaveChanges();  



                }else{

                    vendaLoc.VendaProdutos = vendaLoc.VendaProdutos;
                    
                    
                }

                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult (new{msg="Venda não localizada!"}); 


                }

               

            }    
            Response.StatusCode = 200;
            return new ObjectResult (new{msg="Venda alterada com sucesso!"}); 


        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id){

               try{
                var vendas = database.Vendas.First(f=> f.Id == id);
                database.Vendas.Remove(vendas);
                database.SaveChanges();
               
            return Ok("Venda excluída com sucesso"); 
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        } 

         public class VendasGetContainer{

            [JsonIgnore]
            public Venda vendas {get; set;}

            public int Id { get; set; }

            public int ClienteId {get; set;}

            public List<int> ProdutosId {get; set;}

            public List<int> FornecedorId{get; set;}

            public double TotalCompra {get; set;}

            public string DataCompra{get; set;}

            public Link[] links {get; set;}
        }

         


    }
        
 }
