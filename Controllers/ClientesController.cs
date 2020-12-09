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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CryptSharp;
using Newtonsoft.Json;




namespace Desafio_API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]

    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;



        public ClientesController(ApplicationDbContext database){
            this.database = database;

            HATEOAS = new HATEOAS.HATEOAS ("localhost:5001/api/v1/clientes");
            HATEOAS.AddAction("GET_LIST", "GET");
            HATEOAS.AddAction("DELETE_CLIENTE", "DELETE");
            HATEOAS.AddAction("EDIT_CLIENTE", "PATCH");

            
        }
        [HttpGet]   
        public IActionResult ListaClientes (){
            var clientes = database.Clientes.ToList();
            
            
            
            
             List<ClientesGetContainer> clientesHATEOAS = new List<ClientesGetContainer>();
            foreach(var cliente in clientes){
                ClientesGetContainer clienteHateoas = new ClientesGetContainer();
                clienteHateoas.Id = cliente.Id;
                clienteHateoas.Nome = cliente.Nome;
                clienteHateoas.Email = cliente.Email;
                clienteHateoas.Documento = cliente.Documento;
                clienteHateoas.DataCadastro = Convert.ToString(cliente.DataCadastro).Substring(0,10);
                clienteHateoas.links = HATEOAS.GetActions(cliente.Id.ToString());
                clientesHATEOAS.Add(clienteHateoas);


             
            }
            return Ok(new{clientesHATEOAS});
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClienteDTO cDTO){
            Cliente cliente = new Cliente();

              if(cDTO.Nome.Length <= 4){
                Response.StatusCode = 400;
                return new ObjectResult (new{msg="Favor inserir um nome válido, com pelo menos 5 caracteres!"});
                }

                try{
                var emailVerifique = database.Clientes.First(e=> e.Email == cDTO.Email);
                if(emailVerifique.Email == cDTO.Email){
                    Response.StatusCode = 400;
                    return new ObjectResult (new{msg="Esse e-mail já existe em nosso banco de dados! Nome: " + emailVerifique.Nome});                    

                }

                }catch{
                     if (ClienteDTO.ValidarEmail(cDTO.Email) == false)
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult (new{msg="Favor inserir um email válido!"});
                    }    

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
                var senhaCriptografada = CalculaHash(cDTO.Senha);
                cliente.Senha = senhaCriptografada;
                cliente.Documento = cDTO.Documento;
                cliente.DataCadastro = DateTime.Now;
                

            
            database.Clientes.Add(cliente);
            database.SaveChanges();                     

            
            Response.StatusCode = 201;
            return new ObjectResult (new{msg = "Cliente cadastrado com sucesso!" });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                var clientes = database.Clientes.First(f=> f.Id == id);
                ClientesContainer clienteHATEOAS = new ClientesContainer();
                clienteHATEOAS.Id = clientes.Id;
                clienteHATEOAS.Nome = clientes.Nome;
                clienteHATEOAS.Email = clientes.Email;
                clienteHATEOAS.Documento = clientes.Documento;       
                clienteHATEOAS.DataCadastro = Convert.ToString(clientes.DataCadastro).Substring(0,10);
                clienteHATEOAS.links = HATEOAS.GetActions(clientes.Id.ToString());

                             
            return Ok(clienteHATEOAS);
               
            }catch(Exception ){  

            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Id inválido"}); }

        }

        [HttpPut]
        public IActionResult Put ([FromBody] ClienteDTO cliente){           
           
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

        [HttpGet("asc")]   
        public IActionResult ListaAlfCres(){
            var clientes = database.Clientes.OrderBy(c=> c.Nome).ToList();

            List<ClientesContainer> clientesHATEOAS = new List<ClientesContainer>();
            foreach(var clienteH in clientes){
                ClientesContainer clienteHateoas = new ClientesContainer();
                clienteHateoas.Id = clienteH.Id;
                clienteHateoas.Nome = clienteH.Nome;
                clienteHateoas.Email = clienteH.Email;
                clienteHateoas.Documento = clienteH.Documento;       
                clienteHateoas.DataCadastro = Convert.ToString(clienteH.DataCadastro).Substring(0,10);
                clienteHateoas.links = HATEOAS.GetActions(clienteH.Id.ToString());
                clientesHATEOAS.Add(clienteHateoas);             
            }

                        
           return Ok(new{clientesHATEOAS}); 
        }

        [HttpGet("desc")]   
        public IActionResult ListaAlfDec(){
            var clientes = database.Clientes.OrderByDescending(c=> c.Nome).ToList();

             List<ClientesContainer> clientesHATEOAS = new List<ClientesContainer>();
            foreach(var clienteH in clientes){
                ClientesContainer clienteHateoas = new ClientesContainer();
                clienteHateoas.Id = clienteH.Id;
                clienteHateoas.Nome = clienteH.Nome;
                clienteHateoas.Email = clienteH.Email;
                clienteHateoas.Documento = clienteH.Documento;       
                clienteHateoas.DataCadastro = Convert.ToString(clienteH.DataCadastro).Substring(0,10);
                clienteHateoas.links = HATEOAS.GetActions(clienteH.Id.ToString());
                clientesHATEOAS.Add(clienteHateoas);                        
            } 
             
           return Ok(new{clientesHATEOAS}); 
        }
        
        [HttpGet("nome/{nome}")]   
        public IActionResult PesquisaNome(string nome){
            try{
            var cliente = database.Clientes.Where(c=> c.Nome.Contains(nome)).ToList();
           List<ClientesContainer> clientesHATEOAS = new List<ClientesContainer>();
            foreach(var clienteH in cliente){
                ClientesContainer clienteHateoas = new ClientesContainer();
                clienteHateoas.Id = clienteH.Id;
                clienteHateoas.Nome = clienteH.Nome;
                clienteHateoas.Email = clienteH.Email;
                clienteHateoas.Documento = clienteH.Documento;       
                clienteHateoas.DataCadastro = Convert.ToString(clienteH.DataCadastro).Substring(0,10);
                clienteHateoas.links = HATEOAS.GetActions(clienteH.Id.ToString());
                clientesHATEOAS.Add(clienteHateoas);                        
            }          
        
             
            
            if(cliente.Count == 0){
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de clientes"}); }
               
             
           return Ok(new{clientesHATEOAS}); 
           }catch{
            Response.StatusCode = 404;          
            return new ObjectResult (new{msg= "Nome não disponível na lista de clientes"}); }
           
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] Cliente credenciais){

        try{

        Cliente usuario = database.Clientes.First(user=> user.Email.Equals(credenciais.Email));
        var senhaCriptografada = CalculaHash(credenciais.Senha);
    

        
        if(usuario != null){
            if(senhaCriptografada.Equals(usuario.Senha)){

            string chaveSeguranca = "desafioapi_chave_seguranca_estudos_gft";
            var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSeguranca));
            var credenciaisDeAcesso = new SigningCredentials (chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            claims.Add(new Claim("id", usuario.Id.ToString()));
            claims.Add(new Claim("email", usuario.Email));
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            var JWT = new JwtSecurityToken(

              issuer: "DesafioApi",
              expires: DateTime.Now.AddHours(1),
              audience: "usuario_comum",
              signingCredentials: credenciaisDeAcesso,
              claims: claims
            );


                return Ok (new JwtSecurityTokenHandler().WriteToken(JWT));
            
            
            }else{
            Response.StatusCode = 401;
            return new ObjectResult ("Senha incorreta");
                

            }

        }else{
            Response.StatusCode = 401;
            return new ObjectResult ("Cliente não cadastrado");
        }


            }catch{
            Response.StatusCode = 401;
            return new ObjectResult ("Cliente ou senha incorretos ou não cadastrados!");

            }
        }



        
         public class ClientesGetContainer{

            [JsonIgnore]
            public Cliente clientes {get; set;}

            public int Id { get; set; }        
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Documento { get; set; }

            public string DataCadastro{get; set;}

            public Link[] links {get; set;}
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


        public class ClientesContainer{
            
            [JsonIgnore]
            public Cliente clientes {get; set;}

            public int Id { get; set; }        
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Documento { get; set; }

            public string DataCadastro{get; set;}


            public Link[] links {get; set;}
        }

        
    }

    
}