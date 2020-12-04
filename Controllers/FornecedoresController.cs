using Microsoft.AspNetCore.Mvc;
using Desafio_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Desafio_API.Models;
using Desafio_API.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace Desafio_API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class FornecedoresController : ControllerBase
    {
         

        private readonly ApplicationDbContext database;

        public FornecedoresController(ApplicationDbContext database){
            this.database = database;
            
        }
        [HttpGet]   
        public IActionResult ListaFornecedores (){
            var fornecedores = database.Fornecedores.Include(f=> f.Produtos).ToList();
             
           return Ok(new{fornecedores}); 
        }
         [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                var fornecedores = database.Fornecedores.Include(p=> p.Produtos).First(f=> f.Id == id);
                             
            return Ok(fornecedores);
               
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        }

        
        [HttpGet("asc")]   
        public IActionResult ListaAlfCres(){
            var fornecedores = database.Fornecedores.ToList();

            IEnumerable<Fornecedor> fornecedor = from word in fornecedores 
                            orderby word.Nome
                            select word;  
  
            foreach (var str in fornecedor)  {

            }
             
             
           return Ok(new{fornecedor}); 
        }

         [HttpGet("desc")]   
        public IActionResult ListaAlfDec(){
            var fornecedores = database.Fornecedores.ToList();

            IEnumerable<Fornecedor> fornecedor = from word in fornecedores 
                            orderby word.Nome descending  
                            select word;  
  
            foreach (var str in fornecedor)  {

            }
             
             
           return Ok(new{fornecedor}); 
        }

         [HttpGet("nome/{nome}")]   
        public IActionResult PesquisaNome(string nome){
            try{
            var fornecedor= database.Fornecedores.Where(f=> f.Nome.Contains(nome)).ToList();
            if(fornecedor.Count == 0){
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de fornecedores"}); }
               
             
           return Ok(new{fornecedor}); 
           }catch{
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de fornecedores"}); }
           
        }



        [HttpPost]
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

          [HttpPatch]
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



    }
        
}
