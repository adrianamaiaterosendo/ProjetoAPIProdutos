using System.Collections.Generic;
using Newtonsoft.Json;


namespace Desafio_API.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public double Valor { get; set; }
        public bool Promocao { get; set; }
        public double ValorPromocao { get; set; }
        public string Categoria { get; set; }
        public string Imagem { get; set; }
        public int Quantidade{get; set;}
        public Fornecedor Fornecedor {get; set;}

        [JsonIgnore]
        public ICollection <VendaProduto> VendaProdutos{get; set;}
       

      

    }
}