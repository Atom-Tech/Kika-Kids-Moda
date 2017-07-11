using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbVenda")]
    public class Venda
    {
        public async Task Load()
        {
            if (CodigoProduto != null)
            {
                Produto = (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == CodigoProduto).FirstOrDefault();
            }
            if (CPFCliente != null)
            {
                Cliente = (await Synchro.tbCliente.ReadAsync()).Where(v => v.CPF == CPFCliente).FirstOrDefault();
            }
        }

        public string Id { get; set; }
        [JsonProperty(PropertyName = "idProduto")]
        public string CodigoProduto { get; set; }
        [JsonProperty(PropertyName = "cpfCliente")]
        public string CPFCliente { get; set; }
        [JsonIgnore]
        public Produto Produto { get; set; }
        [JsonIgnore]
        public Cliente Cliente { get; set; }
        [JsonProperty(PropertyName = "qtProduto")]
        public int QuantidadeProduto { get; set; }
        [JsonProperty(PropertyName = "vlVenda")]
        public double Valor { get; set; }
        [JsonProperty(PropertyName = "dtVenda")]
        public string Data { get; set; }
        [JsonProperty(PropertyName = "dsFormaPagamento")]
        public string FormaPagamento { get; set; }
        [JsonProperty(PropertyName = "vlEntrada")]
        public double ValorEntrada { get; set; }
        [Version]
        public byte[] Version { get; set; }
        public static Dictionary<string, string> Colunas { get; } = new Dictionary<string, string>()
        {
            { "CPFCliente", "CPF do Cliente" },
            { "CodigoProduto", "Código do Produto" },
            { "QuantidadeProduto", "Quantidade do Produto" },
            { "FormaPagamento", "Forma de Pagamento" },
            { "ValorEntrada", "Valor de Entrada" }
        };
    }
}
