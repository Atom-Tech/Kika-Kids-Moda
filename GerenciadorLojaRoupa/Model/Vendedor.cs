using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbVendedor")]
    public class Vendedor
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "cpfVendedora")]
        public string CPF { get; set; }
        [JsonProperty(PropertyName = "nmVendedora")]
        public string Nome { get; set; }
        [JsonProperty(PropertyName = "dsEndereco")]
        public string Endereco { get; set; }
        [JsonProperty(PropertyName = "rgVendedora")]
        public string RG { get; set; }
        [JsonProperty(PropertyName = "nrTelefone")]
        public string Telefone { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}
