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

    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public ClientesController(ApplicationDbContext database){
            this.database = database;
            
        }
        [HttpGet]   
        public IActionResult ListaClientes (){
            var clientes = database.Clientes.ToList();
             
           return Ok(new{clientes}); 
        }

        [HttpGet("asc")]   
        public IActionResult ListaAlfCres(){
            var clientes = database.Clientes.ToList();

            IEnumerable<Cliente> cliente = from word in clientes 
                            orderby word.Nome
                            select word;  
  
            foreach (var str in cliente)  {

            }
             
             
           return Ok(new{cliente}); 
        }

         [HttpGet("desc")]   
        public IActionResult ListaAlfDec(){
            var clientes = database.Clientes.ToList();

            IEnumerable<Cliente> cliente = from word in clientes 
                            orderby word.Nome descending  
                            select word;  
  
            foreach (var str in cliente)  {

            }
             
             
           return Ok(new{cliente}); 
        }




           [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                var clientes = database.Clientes.First(f=> f.Id == id);
                             
            return Ok(clientes);
               
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        }

        
        [HttpGet("nome/{nome}")]   
        public IActionResult PesquisaNome(string nome){
            try{
            var cliente = database.Clientes.Where(c=> c.Nome.Contains(nome)).ToList();
            if(cliente.Count == 0){
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de clientes"}); }
               
             
           return Ok(new{cliente}); 
           }catch{
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de clientes"}); }
           
        }



        [HttpPost]
        public IActionResult Post([FromBody] ClienteDTO cDTO){
            Cliente cliente = new Cliente();

              if(cDTO.Nome.Length <= 4){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um nome válido, com pelo menos 5 caracteres!"});
                }
              if (cDTO.Email.Length <= 8){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um email válido, com pelo menos 8 caracteres!"});
                }
              if(cDTO.Senha.Length < 6){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir uma senha válida, com no mínimo 6 caracteres!"});
                } 
                 if(cDTO.Documento.Length < 8){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um documento válido, com no mínimo 8 caracteres!"});
                } 
                cliente.Nome =  cDTO.Nome;
                cliente.Email = cDTO.Email;
                cliente.Senha = cDTO.Senha;
                cliente.Documento = cDTO.Documento;
                cliente.DataCadastro = DateTime.Now;
                

            
            database.Clientes.Add(cliente);
            database.SaveChanges();                     

            
            Response.StatusCode = 201;
            return new ObjectResult (new{msg = "Cliente cadastrado com sucesso!" });
        }

        [HttpPatch]
        public IActionResult Editar ([FromBody] ClienteDTO cliente){           
           
            if(cliente.Id > 0){
                try{ 

                    var clienteTemp = database.Clientes.First(c => c.Id == cliente.Id);
                    
                            

                    if(clienteTemp != null){

                        if(cliente.Nome != null ){
                            if(cliente.Nome.Length <= 2){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Nome inválido ou vazio, tente outro nome!"}); 
                            }
                            clienteTemp.Nome = cliente.Nome;
                            database.SaveChanges();
                        }else{
                            clienteTemp.Nome = clienteTemp.Nome;
                            database.SaveChanges();
                        }

                           if(cliente.Email != null ){
                            if(cliente.Email.Length < 8){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um email válido, com pelo menos 8 caracteres!"}); 
                            }
                            clienteTemp.Email = cliente.Email;
                            database.SaveChanges();
                        }else{
                            clienteTemp.Email = clienteTemp.Email;
                            database.SaveChanges();
                        }
                          if(cliente.Senha != null ){
                            if(cliente.Senha.Length < 6){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir uma senha válida, com pelo menos 6 caracteres!"}); 
                            }
                            clienteTemp.Senha = cliente.Senha;
                            database.SaveChanges();
                        }else{
                            clienteTemp.Senha = clienteTemp.Senha;
                            database.SaveChanges();
                        }
                         if(cliente.Documento != null ){
                            if(cliente.Documento.Length < 8){
                                Response.StatusCode = 400;
                                return new ObjectResult (new{msg="Favor inserir um documento válido, com no mínimo 8 caracteres!"}); 
                            }
                            clienteTemp.Documento = cliente.Documento;
                            database.SaveChanges();
                        }else{
                            clienteTemp.Documento = clienteTemp.Documento;
                            database.SaveChanges();
                        }                    
                 
                       return Ok("Alterações efetuadas com sucesso!");
                }}catch{

                    Response.StatusCode = 404;
                    return new ObjectResult (new{msg="Cliente não localizado"}); 

                }

            }if(cliente == null){
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="Cliente não encontrado"}); }
            Response.StatusCode = 404;
            return new ObjectResult (new{msg="Cliente não encontrado"}); 
               
            }     


        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id){

               try{
                var clientes = database.Clientes.First(f=> f.Id == id);
                database.Clientes.Remove(clientes);
                database.SaveChanges();
               
            return Ok("Cliente excluído com sucesso"); 
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        } 
    }
}