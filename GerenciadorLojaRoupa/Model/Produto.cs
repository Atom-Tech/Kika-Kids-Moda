using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbProduto")]
    public class Produto
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "cdProduto")]
        public string Codigo { get; set; }
        [JsonProperty(PropertyName = "nmProduto")]
        public string Nome { get; set; }
        [JsonProperty(PropertyName = "dsProduto")]
        public string Descricao { get; set; }
        [JsonProperty(PropertyName = "tmProduto")]
        public string Tamanho { get; set; }
        [JsonProperty(PropertyName = "qtProduto")]
        public int Quantidade { get; set; }
        [JsonProperty(PropertyName = "vlProduto")]
        public double Valor { get; set; }

        public static async Task<string> GetNome(string codigo)
        {
            return (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == codigo).First().Nome;
        }
    }
}
