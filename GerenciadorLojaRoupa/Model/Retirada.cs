using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbRetirada")]
    public class Retirada
    { 
        public async Task Load()
        {
            if (CodigoProduto != null)
            {
                Produto = (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == CodigoProduto).First();
            }
            if (CPFVendedor != null)
            {
                Vendedor = (await Synchro.tbVendedor.ReadAsync()).Where(v => v.CPF == CPFVendedor).First();
            }
        }

        public async Task<Produto> GetProduto()
        {
            if (CodigoProduto != null)
            {
                return (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == CodigoProduto).First();
            }
            return null;
        }

        public string Id { get; set; }
        [JsonProperty(PropertyName = "idProduto")]
        public string CodigoProduto { get; set; }
        [JsonProperty(PropertyName = "cpfVendedora")]
        public string CPFVendedor { get; set; }
        [JsonIgnore]
        public Produto Produto { get; set; }
        [JsonIgnore]
        public Vendedor Vendedor { get; set; }
        [JsonProperty(PropertyName = "dtRetirada")]
        public string Data { get; set; }
        [JsonProperty(PropertyName = "qtRetirada")]
        public int Quantidade { get; set; }
        [JsonProperty(PropertyName = "retornado")]
        public bool Retornado { get; set; }
    }
}
