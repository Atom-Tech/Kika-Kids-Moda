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
            if (CPFCliente != null)
            {
                Cliente = (await Synchro.tbCliente.ReadAsync()).Where(v => v.CPF == CPFCliente).FirstOrDefault();
            }
        }

        public string Id { get; set; }
        [JsonProperty(PropertyName = "cpfCliente")]
        public string CPFCliente { get; set; }
        [JsonIgnore]
        public Cliente Cliente { get; set; }
        [JsonProperty(PropertyName = "vlVenda")]
        public double Valor { get; set; }
        [JsonProperty(PropertyName = "dtVenda")]
        public string Data { get; set; }
        [JsonProperty(PropertyName = "dsFormaPagamento")]
        public string FormaPagamento { get; set; }
        [JsonProperty(PropertyName = "vlEntrada")]
        public double ValorEntrada { get; set; }
        [JsonProperty(PropertyName = "pctDesconto")]
        public int PorcentagemDesconto { get; set; }
        [JsonProperty(PropertyName = "qtParcelas")]
        public int Parcelas { get; set; }
        [JsonProperty(PropertyName = "vlTotalDesconto")]
        public double ValorTotalDesconto { get; set; }
        [Version]
        public byte[] Version { get; set; }
        public static Dictionary<string, string> Colunas { get; } = new Dictionary<string, string>()
        {
            { "CPFCliente", "CPF do Cliente" },
            { "FormaPagamento", "Forma de Pagamento" },
            { "ValorEntrada", "Valor de Entrada" },
            { "ValorTotalDesconto", "Valor Total com Desconto" },
            { "PorcentagemDesconto", "Porcentagem de Desconto" },
            { "DataPrestacao", "Data da Prestação" },
            { "Pago", "Pago?" }
        };
        [JsonProperty(PropertyName = "pago")]
        public bool Pago { get; set; }
        [JsonProperty(PropertyName = "dtPrestacao")]
        public string DataPrestacao { get; set; }


        public static async Task<string> GetVenda(string codigo)
        {
            return (await Synchro.tbVenda.ReadAsync()).Where(c => c.Id == codigo).First().CPFCliente;
        }
    }
}
