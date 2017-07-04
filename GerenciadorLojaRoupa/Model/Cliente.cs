using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbCliente")]
    public class Cliente
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "cpfCliente")]
        public string CPF { get; set; } //Unique
        [JsonProperty(PropertyName = "nmCliente")] 
        public string Nome { get; set; } //50
        [JsonProperty(PropertyName = "dsEndereco")] 
        public string Endereco { get; set; } //40
        [JsonProperty(PropertyName = "rgCliente")] 
        public string RG { get; set; }
        [JsonProperty(PropertyName = "nrTelefone")] 
        public string Telefone { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; } //Unique 20

        public static async Task<string> GetNome(string codigo)
        {
            return (await Synchro.tbCliente.ReadAsync()).Where(c => c.CPF == codigo).First().Nome;
        }
    }
}
