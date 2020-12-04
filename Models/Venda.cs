using System;
using System.Collections.Generic;


namespace Desafio_API.Models
{
    public class Venda
    {
        public int Id { get; set; }

        public Fornecedor Fornecedor{get; set;}

        public Cliente Cliente {get; set;}

        public double TotalCompra {get; set;}

        public DateTime DataCompra{get; set;}
    
       public ICollection <VendaProduto> VendaProdutos{get; set;}

       public ICollection <VendaFornecedor> VendaFornecedores{get; set;}

       
    }
}