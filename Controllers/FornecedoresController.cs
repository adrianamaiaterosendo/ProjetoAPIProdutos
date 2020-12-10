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
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;


namespace Desafio_API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class FornecedoresController : ControllerBase
    {
         

        private readonly ApplicationDbContext database;

        private HATEOAS.HATEOAS HATEOAS;

        public FornecedoresController(ApplicationDbContext database){
            this.database = database;
            
            HATEOAS = new HATEOAS.HATEOAS ("localhost:5001/api/v1/fornecedores");
            HATEOAS.AddAction("GET_LIST", "GET");
            HATEOAS.AddAction("DELETE_FORNECEDOR", "DELETE");
            HATEOAS.AddAction("EDIT_FORNECEDOR", "PATCH");
            
        }
        
        [HttpGet]   
        public IActionResult ListaFornecedores (){
            var fornecedores = database.Fornecedores.Include(f=> f.Produtos).ToList();

             

            List<FornecedorGetContainer> fornecedoresHATEOAS = new List<FornecedorGetContainer>();
            foreach(var fornecedor in fornecedores){
            FornecedorGetContainer fornecedorHateoas = new FornecedorGetContainer();
            fornecedorHateoas.fornecedorId = fornecedor.Id;
            fornecedorHateoas.fornecedorCNPJ = fornecedor.CNPJ;
            fornecedorHateoas.fornecedorNome = fornecedor.Nome;
            var prodId = database.Produtos.Where(p=> p.Fornecedor.Id == fornecedor.Id).ToList();
            
            List<int> idProd = new List<int>();
            foreach(var prod in prodId){
                idProd.Add(prod.Id);
            }

            fornecedorHateoas.ProdutoId = idProd;
            
            fornecedorHateoas.links = HATEOAS.GetActions(fornecedor.Id.ToString());
            fornecedoresHATEOAS.Add(fornecedorHateoas);}
             
           return Ok(new{fornecedoresHATEOAS}); 
           
        }
       
        [HttpPost]
        [Authorize]    

        public IActionResult Post([FromBody] FornecedorDTO fDTO){
            Fornecedor fornecedor = new Fornecedor();

              if(fDTO.Nome.Length <= 4){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um nome válido, com pelo menos 5 caracteres!"});
                }
              if (fDTO.CNPJ.Length <= 13){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um CNPJ válido, com pelo menos 13 caracteres!"});
                }
                fornecedor.Nome =  fDTO.Nome;
                fornecedor.CNPJ = fDTO.CNPJ;
                            
            database.Fornecedores.Add(fornecedor);
            database.SaveChanges();   

           
            Response.StatusCode = 201;
            return new ObjectResult (new{msg = "Fornecedor cadastrado com sucesso!" });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                var fornecedores = database.Fornecedores.Include(p=> p.Produtos).First(f=> f.Id == id);
                FornecedorContainer fornecedorHATEOAS = new FornecedorContainer();
                fornecedorHATEOAS.Id = fornecedores.Id;
                fornecedorHATEOAS.Nome = fornecedores.Nome;
                fornecedorHATEOAS.CNPJ = fornecedores.CNPJ;

                 var prodId = database.Produtos.Where(p=> p.Fornecedor.Id == fornecedores.Id).ToList();
            
                List<int> idProd = new List<int>();
                foreach(var prod in prodId){
                    idProd.Add(prod.Id);
                }

                fornecedorHATEOAS.ProdutoId = idProd;
            


                fornecedorHATEOAS.links = HATEOAS.GetActions(fornecedores.Id.ToString());
                             
            return Ok(fornecedorHATEOAS);
               
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        }

        [HttpPut]
        [Authorize]    

        public IActionResult Put ([FromBody] FornecedorDTO fornecedor){           
           
            if(fornecedor.Id > 0){
                try{ 

                    var fornecedorTemp = database.Fornecedores.First(f => f.Id == fornecedor.Id);
                   
                            

                    if(fornecedorTemp != null){

                        if(fornecedor.Nome != null ){
                            if(fornecedor.Nome.Length <= 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Nome inválido ou vazio, tente outro nome!"}); 
                            }
                            fornecedorTemp.Nome = fornecedor.Nome;
                            database.SaveChanges();
                        }else{
                            fornecedorTemp.Nome = fornecedorTemp.Nome;
                            database.SaveChanges();
                        }

                           if(fornecedor.CNPJ != null ){
                            if(fornecedor.CNPJ.Length < 13){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um CNPJ válido, com pelo menos 12 caracteres!"}); 
                            }
                            fornecedorTemp.CNPJ = fornecedor.CNPJ;
                            database.SaveChanges();
                        }else{
                            fornecedorTemp.CNPJ = fornecedorTemp.CNPJ;
                            database.SaveChanges();
                        }
                               
                 
                       return Ok("Alterações efetuadas com sucesso!");
                }}catch{

                    Response.StatusCode = 404;
                    return new ObjectResult (new{msg="Fornecedor não localizado"}); 

                }

            }if(fornecedor == null){
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="Fornecedor não encontrado"}); }
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="Fornecedor não encontrado"}); 
               
            }  

        [HttpPatch]
        [Authorize]    

        public IActionResult Editar ([FromBody] FornecedorDTO fornecedor){           
           
            if(fornecedor.Id > 0){
                try{ 

                    var fornecedorTemp = database.Fornecedores.First(f => f.Id == fornecedor.Id);
                   
                            

                    if(fornecedorTemp != null){

                        if(fornecedor.Nome != null ){
                            if(fornecedor.Nome.Length <= 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Nome inválido ou vazio, tente outro nome!"}); 
                            }
                            fornecedorTemp.Nome = fornecedor.Nome;
                            database.SaveChanges();
                        }else{
                            fornecedorTemp.Nome = fornecedorTemp.Nome;
                            database.SaveChanges();
                        }

                           if(fornecedor.CNPJ != null ){
                            if(fornecedor.CNPJ.Length < 13){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um CNPJ válido, com pelo menos 12 caracteres!"}); 
                            }
                            fornecedorTemp.CNPJ = fornecedor.CNPJ;
                            database.SaveChanges();
                        }else{
                            fornecedorTemp.CNPJ = fornecedorTemp.CNPJ;
                            database.SaveChanges();
                        }
                               
                 
                       return Ok("Alterações efetuadas com sucesso!");
                }}catch{

                    Response.StatusCode = 404;
                    return new ObjectResult (new{msg="Fornecedor não localizado"}); 

                }

            }if(fornecedor == null){
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="Fornecedor não encontrado"}); }
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="Fornecedor não encontrado"}); 
               
            }  

        [HttpDelete("{id}")]
        [Authorize]    

        public IActionResult Delete(int id){

               try{
                var fornecedores = database.Fornecedores.First(f=> f.Id == id);
                database.Fornecedores.Remove(fornecedores);
                database.SaveChanges();
               
            return Ok("Fornecedor excluído com sucesso"); 
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        } 

        [HttpGet("asc")]   
        public IActionResult ListaAlfCres(){
            var fornecedores = database.Fornecedores.OrderBy(f=> f.Nome).ToList();
             
            List<FornecedorContainer> fornecedoresHATEOAS = new List<FornecedorContainer>();
            foreach(var fornecedor in fornecedores){
                FornecedorContainer fornecedorHATEOAS = new FornecedorContainer();
                fornecedorHATEOAS.Id = fornecedor.Id;
                fornecedorHATEOAS.Nome = fornecedor.Nome;
                fornecedorHATEOAS.CNPJ = fornecedor.CNPJ;

                 var prodId = database.Produtos.Where(p=> p.Fornecedor.Id == fornecedor.Id).ToList();
            
                List<int> idProd = new List<int>();
                foreach(var prod in prodId){
                    idProd.Add(prod.Id);
                }

                fornecedorHATEOAS.ProdutoId = idProd;
                fornecedorHATEOAS.links = HATEOAS.GetActions(fornecedor.Id.ToString());
                fornecedoresHATEOAS.Add(fornecedorHATEOAS);

            }return Ok(new{fornecedoresHATEOAS}); 
        }

        [HttpGet("desc")]   
        public IActionResult ListaAlfDec(){
            var fornecedores = database.Fornecedores.OrderByDescending(f=> f.Nome).ToList();
             
            List<FornecedorContainer> fornecedoresHATEOAS = new List<FornecedorContainer>();
            foreach(var fornecedor in fornecedores){
                FornecedorContainer fornecedorHATEOAS = new FornecedorContainer();
                fornecedorHATEOAS.Id = fornecedor.Id;
                fornecedorHATEOAS.Nome = fornecedor.Nome;
                fornecedorHATEOAS.CNPJ = fornecedor.CNPJ;

                 var prodId = database.Produtos.Where(p=> p.Fornecedor.Id == fornecedor.Id).ToList();
            
                List<int> idProd = new List<int>();
                foreach(var prod in prodId){
                    idProd.Add(prod.Id);
                }

                fornecedorHATEOAS.ProdutoId = idProd;
                fornecedorHATEOAS.links = HATEOAS.GetActions(fornecedor.Id.ToString());
                fornecedoresHATEOAS.Add(fornecedorHATEOAS);

            }return Ok(new{fornecedoresHATEOAS}); 
        }


        [HttpGet("nome/{nome}")]   
        public IActionResult PesquisaNome(string nome){
            try{
            var fornecedores= database.Fornecedores.Where(f=> f.Nome.Contains(nome)).ToList();
             List<FornecedorContainer> fornecedoresHATEOAS = new List<FornecedorContainer>();
            foreach(var fornecedor in fornecedores){
                FornecedorContainer fornecedorHATEOAS = new FornecedorContainer();
                fornecedorHATEOAS.Id = fornecedor.Id;
                fornecedorHATEOAS.Nome = fornecedor.Nome;
                fornecedorHATEOAS.CNPJ = fornecedor.CNPJ;

                 var prodId = database.Produtos.Where(p=> p.Fornecedor.Id == fornecedor.Id).ToList();
            
                List<int> idProd = new List<int>();
                foreach(var prod in prodId){
                    idProd.Add(prod.Id);
                }

                fornecedorHATEOAS.ProdutoId = idProd;
                fornecedorHATEOAS.links = HATEOAS.GetActions(fornecedor.Id.ToString());
                fornecedoresHATEOAS.Add(fornecedorHATEOAS);}

            if(fornecedores.Count == 0){
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de fornecedores"}); }
                           
           return Ok(new{fornecedoresHATEOAS}); 
           }catch{
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de fornecedores"}); }
           
        }


         public class FornecedorGetContainer{

            [JsonIgnore]
            public Fornecedor fornecedores {get; set;}
         
            public int fornecedorId{get;set; }
            public string fornecedorNome{get;set; }
            public string fornecedorCNPJ{get;set; }
            public List<int> ProdutoId{get;set; }

            public Link[] links {get; set;}
         }

        public class FornecedorContainer{ 

            [JsonIgnore]          
            public Fornecedor fornecedores {get; set;}
         
            public int Id{get;set; }
            public string Nome{get;set; }
            public string CNPJ{get;set; }
            public List<int> ProdutoId{get;set; }

            public Link[] links {get; set;}

          
        }

       



    }
        
}

