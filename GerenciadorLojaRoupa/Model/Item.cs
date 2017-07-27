using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbItem")]
    public class Item
    {
        public async Task Load()
        {
            if (CodigoProduto != null)
            {
                Produto = (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == CodigoProduto).FirstOrDefault();
            }
            if (CodigoVenda != null)
            {
                Venda = (await Synchro.tbVenda.ReadAsync()).Where(v => v.Id == CodigoVenda).FirstOrDefault();
            }
        }

        public string Id { get; set; }
        [JsonProperty(PropertyName = "idProduto")]
        public string CodigoProduto { get; set; }
        [JsonProperty(PropertyName ="idVenda")]
        public string CodigoVenda { get; set; }
        [JsonIgnore]
        public Venda Venda { get; set; }
        [JsonIgnore]
        public Produto Produto { get; set; }
        [JsonProperty(PropertyName = "qtProduto")]
        public int QuantidadeProduto { get; set; }
        [JsonProperty(PropertyName = "vlProduto")]
        public double ValorProduto { get; set; }
        [Version]
        public byte[] Version { get; set; }
    }
}
