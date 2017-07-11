using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbUsuario")]
    public class Usuario
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "loginUsuario")]
        public string Login { get; set; }
        [JsonProperty(PropertyName = "senha")]
        public string Senha { get; set; }
        [JsonProperty(PropertyName = "nmUsuario")]
        public string Nome { get; set; }
        [JsonProperty(PropertyName = "dsEndereco")]
        public string Endereco { get; set; }
        [JsonProperty(PropertyName = "cpfUsuario")]
        public string CPF { get; set; }
        [JsonProperty(PropertyName = "nvAcesso")]
        public string NivelAcesso { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [Version]
        public byte[] Version { get; set; }
    }
}
