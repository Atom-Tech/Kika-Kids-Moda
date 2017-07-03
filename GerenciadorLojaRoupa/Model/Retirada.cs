using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public string Id { get; set; }
        [JsonProperty(PropertyName = "idProduto")]
        public string CodigoProduto { get; set; }
        [JsonProperty(PropertyName = "cpfVendedora")]
        public string CPFVendedor { get; set; }
        [JsonIgnore]
        public string NomeVendedor
        {
            get
            {
                return Vendedor.Nome;
            }
        }
        [JsonIgnore]
        public string NomeProduto
        {
            get
            {
                return Produto.Nome;
            }
        }
        [JsonIgnore]
        public Produto Produto { get; set; }
        [JsonIgnore]
        public Vendedor Vendedor { get; set; }
        [JsonProperty(PropertyName = "dtRetirada")]
        public string Data { get; set; }
        [JsonProperty(PropertyName = "qtRetirada")]
        public int Quantidade { get; set; }
        [JsonProperty(PropertyName = "vlUnit")]
        public double ValorUnitario { get; set; }
        [JsonProperty(PropertyName = "vlTotal")]
        public double ValorTotal { get; set; }
        [JsonProperty(PropertyName = "retornado")]
        public bool Retornado { get; set; }
    }
}
