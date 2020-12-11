using System;
using System.Collections.Generic;

namespace Desafio_API.DTO
{
    public class VendaDTO
    {

        
        public int Id { get; set; }

        //public int FornecedorId{get; set;}

        public int ClienteId {get; set;}

        public List<int> ProdutosId {get; set;}

        public double TotalCompra {get; set;}

        public DateTime DataCompra{get; set;}
        
    }

   
}