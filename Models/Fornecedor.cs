using System.Collections.Generic;

namespace Desafio_API.Models
{
    public class Fornecedor
    {      
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ {get; set;}

        public ICollection <Produto> Produtos {get; set;}

        
       
        
    }
}