using Desafio_API.Models;
using System.Collections.Generic;

namespace Desafio_API.DTO
{
    public class FornecedorDTOSaida
    {
        
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ {get; set;}
        public List<Produto> ProdutosId {get; set;}

        public List <Fornecedor> fornecedorLista {get; set;}

        
        

        
    }
}