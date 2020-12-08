using System;

namespace Desafio_API.DTO
{
    public class ClienteDTO
    {

        public int Id { get; set; }        
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Documento { get; set; }

        public DateTime DataCadastro{get; set;}


        public static bool ValidarEmail(string strEmail)
        {
        string strModelo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (System.Text.RegularExpressions.Regex.IsMatch(strEmail,strModelo))
        {
        return true;
        }
        else
        {
        return false;
        }
}
    }
}